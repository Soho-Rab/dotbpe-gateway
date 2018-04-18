using DotBPE.Rpc;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.AspNetGateway.Pipelines
{
    public class ProtocolPipe<TMessage>: IPipeline where TMessage : InvokeMessage
    {
        private readonly ILogger _logger;
        private readonly HttpRouterOption _option;
        private readonly ICallInvoker<TMessage> _invoker;
        private readonly IMessageParser<TMessage> _parser;

        private static ConcurrentDictionary<string, HttpRouterOptionItem> ROUTER_CACHE = new ConcurrentDictionary<string, HttpRouterOptionItem>();

        public ProtocolPipe(
            IOptions<HttpRouterOption> optionsAccessor,
            IMessageParser<TMessage> parser,
            ICallInvoker<TMessage> invoker,
            ILoggerFactory loggerFactory)
        {
            Preconditions.CheckNotNull(optionsAccessor.Value, "WebApiRouterOption");
            Preconditions.CheckNotNull(parser, "IMessageParser");
            Preconditions.CheckNotNull(invoker, "ICallInvoker");

            _invoker = invoker;
            _parser = parser;
            _option = optionsAccessor.Value;
            
            _logger = loggerFactory.CreateLogger(this.GetType());
        }

        public async Task<bool> Invoke(HttpContext context)
        {
            bool result = false;
            using (var httpMetric = CreateHttpMetric())
            {
                result = await ProcessInnerAsync(context.Request, context.Response);
                await httpMetric.AddToMetricsAsync(context.Request, context.Response);
            }
            return result;
        }

        protected IHttpMetric CreateHttpMetric()
        {
            return new NoopHttpMetric();
        }

        /// <summary>
        /// 讲Http请求转成RPC调用，发送到服务端，并接受其响应，并回复给Http请求调用方
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="res">The res.</param>
        /// <returns></returns>
        public async Task<bool> ProcessInnerAsync(HttpRequest req, HttpResponse res)
        {
            bool hasRes = false;
            var router = FindRouter(req);
            if (router == null)
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                await res.WriteAsync("service not found!");
                return false;
            }

            //收集数据
            RequestData rd = new RequestData();
            hasRes = await ParseRequestAsync(req, res, router, rd);

            if (hasRes)
            {
                return hasRes;
            }

            //数据收集成功后，需要收集一些额外的数据
            hasRes = await ParsePostRequestAsync(req, res, router, rd);
            if (hasRes)
            {
                return hasRes;
            }
            hasRes = await BeforeAsyncCall(req, res, rd);
            if (hasRes)
            {
                return hasRes;
            }
            try
            {
                //协议转换
                TMessage reqMsg = this._parser.ToMessage(rd);
                TMessage rspMsg = await _invoker.AsyncCall(reqMsg, router.TimeOut > 0 ? router.TimeOut : 3000);

                if (rspMsg != null)
                {
                    //预处理响应数据
                    hasRes = await BeforeSendResponse(req, res, router, rspMsg);
                    if (hasRes)
                    {
                        return hasRes;
                    }
                    hasRes = await ProccessOutputAsync(req, res, router, rd, rspMsg);
                    return hasRes;
                }
                else
                {
                    this._logger.LogWarning("req serviceId={0},messageId={1} rsp is null", rd.ServiceId, rd.MessageId);
                    res.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await res.WriteAsync("call result is null");
                    return true;
                }
            }
            catch (RpcCommunicationException rpcEx)
            {
                this._logger.LogError(rpcEx, "rpc communication error:");
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                await res.WriteAsync("request timeout");
                return true;
            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                await res.WriteAsync("InternalServerError");
                this._logger.LogError(ex, "call error:");
                return true;
            }
        }

        /// <summary>
        /// 在发送请求前再一次处理，响应消息，一般不用重写
        /// </summary>
        /// <param name="req">request.</param>
        /// <param name="res">repsonse.</param>
        /// <param name="resMessage">The resource message.</param>
        protected async Task<bool> BeforeSendResponse(HttpRequest req, HttpResponse res, HttpRouterOptionItem router, TMessage resMessage)
        {
            SetContentType(req, res, resMessage);
            bool result = false;
            IHttpPostProcessPlugin<TMessage> plugin = null;
            if (router.Plugin != null && router.Plugin is IHttpPostProcessPlugin<TMessage>)
            {
                plugin = router.Plugin as IHttpPostProcessPlugin<TMessage>;
            }

            if (plugin != null)
            {
                result = await plugin.PostProcessAsync(req, res, resMessage, router);
            }

            return result;
        }

        protected virtual void SetContentType(HttpRequest req, HttpResponse res, TMessage resMsg)
        {
            res.ContentType = "application/json;charset=utf-8";
        }

        /// <summary>
        /// 从Context中自定义提取数据到请求字典中
        /// </summary>
        /// <param name="context"></param>
        /// <param name="collDataDict"></param>
        protected virtual void CollectCommonData(HttpRequest request, Dictionary<string, string> collDataDict)
        {
            //添加客户端IP 项目中可根据实际情况添加需要的内容
            string ip = request.GetUserIp();
            if (collDataDict.ContainsKey(Constants.CLIENTIP_FIELD_NAME))
            {
                collDataDict.Remove(Constants.CLIENTIP_FIELD_NAME);
            }
            collDataDict.Add(Constants.CLIENTIP_FIELD_NAME, ip);

            if (collDataDict.ContainsKey(Constants.IDENTITY_FIELD_NAME))
            {
                collDataDict.Remove(Constants.IDENTITY_FIELD_NAME);
            }

            //登录用户标识
            if (request.HttpContext.User.Identity.IsAuthenticated)
            {
                collDataDict.Add(Constants.IDENTITY_FIELD_NAME,
                    request.HttpContext.User.Identity.Name);
                    
                //将所有的Claims 全局加到Dict中
                request.HttpContext.User.Claims.ToList().ForEach(item => collDataDict.Add(item.Type, item.Value));
            }

            //从Head中提取 x-request-id
            var requestId = string.Empty;
            if (request.Headers.ContainsKey(Constants.REQUESTID_HEAD_NAME))
            {
                Microsoft.Extensions.Primitives.StringValues sv;
                bool hasSV = request.Headers.TryGetValue(Constants.REQUESTID_HEAD_NAME, out sv);
                if (hasSV && sv.Count > 0)
                {
                    requestId = sv[0];
                }
            }
            if (string.IsNullOrEmpty(requestId))
            {
                requestId = Guid.NewGuid().ToString("N");
            }
            collDataDict.Add(Constants.REQUESTID_FIELD_NAME, requestId);
        }

        /// <summary>
        /// 在实际调用调用服务前最后的处理
        /// </summary>
        /// <param name="req"></param>
        /// <param name="router"></param>
        /// <param name="rd"></param>
        /// <returns></returns>
        protected virtual Task<bool> BeforeAsyncCall(HttpRequest req, HttpResponse res ,RequestData rd)
        {
            return Task.FromResult(false);
        }
        
        /// <summary>
        /// 从请求中提取请求数据到RequestData中
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual void ProcessRequestData(HttpRequest req, HttpRouterOptionItem router, RequestData rd)
        {
            var method = req.Method.ToLower();

            rd.ServiceId = router.ServiceId;
            rd.MessageId = router.MessageId;         

            CollectQuery(req.Query, rd.Data);
            string contentType = "";
            if (method == "post" || method == "put")
            {
                if (!string.IsNullOrEmpty(req.ContentType))            
                {
                    contentType = req.ContentType.ToLower().Split(';')[0];
                }               
            }

            if (contentType == "application/x-www-form-urlencoded"
                    || contentType == "multipart/form-data"
                    )
            {
                CollectForm(req.Form, rd.Data);
            }

            if (contentType == "application/json")
            {
                rd.RawBody = CollectBody(req.Body);
            }
        }

        private async Task<bool> ProccessOutputAsync(HttpRequest req, HttpResponse res, HttpRouterOptionItem router, RequestData rd, TMessage resultMsg)
        {
            IHttpOutputPlugin<TMessage> plugin = null;
            if (router.Plugin != null && router.Plugin is IHttpOutputPlugin<TMessage>)
            {
                plugin = router.Plugin as IHttpOutputPlugin<TMessage>;
            }

            if (plugin != null)
            {
                var result = await plugin.OutputAsync(req, res, resultMsg, router);
                return result;
            }
            else
            {
                await res.WriteAsync(this._parser.ToJson(resultMsg));
                return true;
            }
        }

        private async Task<bool> ParsePostRequestAsync(HttpRequest req, HttpResponse res, HttpRouterOptionItem router, RequestData rd)
        {
            bool result = false;
            IHttpPostParsePlugin plugin = null;
            if (router.Plugin != null && router.Plugin is IHttpPostParsePlugin)
            {
                plugin = router.Plugin as IHttpPostParsePlugin;
            }

            if (plugin != null)
            {
                result = await plugin.PostParseAsync(req, res, rd, router);
                return result;
            }

            return result;
        }

        private async Task<bool> ParseRequestAsync(HttpRequest req, HttpResponse res, HttpRouterOptionItem router, RequestData rd)
        {
            bool result = false;
            IHttpParsePlugin plugin = null;
            if (router.Plugin != null && router.Plugin is IHttpParsePlugin)
            {
                plugin = router.Plugin as IHttpParsePlugin;
            }

            rd.MessageId = router.MessageId;
            rd.ServiceId = router.ServiceId;
            rd.Data = new Dictionary<string, string>();
            if (plugin != null)
            {              
                result = await plugin.ParseAsync(req, res, rd, router);
                CollectCommonData(req, rd.Data);
                return result;
            }
            else
            {
                try
                {
                    
                    ProcessRequestData(req, router, rd);
                    CollectCommonData(req, rd.Data);
                }
                catch (Exception ex)
                {
                    res.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await res.WriteAsync("InternalServerError:"+ex.Message);
                    _logger.LogError(ex, "转换HTTP请求到RPC请求出错");
                    return true;
                }
            }
         
            return result;
        }

        private HttpRouterOptionItem FindRouter(HttpRequest req)
        {
            string path = req.Path;
            string method = req.Method.ToLower();

            string cacheKey = string.Concat(path, ":", method);
            if (ROUTER_CACHE.ContainsKey(cacheKey))
            {
                return ROUTER_CACHE[cacheKey];
            }
            for (var i = 0; i < _option.Items.Count; i++)
            {
                var router = _option.Items[i];
                // 没有配置Method标识匹配所有请求，否则必须匹配对应的Method
                if (string.IsNullOrEmpty(router.Method)
                    || router.Method.Equals("all", StringComparison.OrdinalIgnoreCase)
                    || router.Method.Equals(method, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Match(router.Path, path);
                    if (match)
                    {
                        ROUTER_CACHE.TryAdd(cacheKey, router);
                        return router;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 路由匹配规则，默认是完全匹配
        /// </summary>
        /// <param name="except">The except.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected virtual bool Match(string except, string value)
        {
            return string.Equals(except, value, StringComparison.OrdinalIgnoreCase);
        }

        private string CollectBody(Stream body)
        {
            string bodyText = null;
            using (StreamReader reader = new StreamReader(body))
            {
                bodyText = reader.ReadToEnd();
            }
            return bodyText;
        }

        private void CollectForm(IFormCollection form, Dictionary<string, string> routeData)
        {
            foreach (string key in form.Keys)
            {
                if (routeData.ContainsKey(key))
                    routeData[key] = form[key];
                else
                    routeData.Add(key, form[key]);
            }
        }

        private void CollectQuery(IQueryCollection query, Dictionary<string, string> routeData)
        {
            foreach (string key in query.Keys)
            {
                if (routeData.ContainsKey(key))
                    routeData[key] = query[key];
                else
                    routeData.Add(key, query[key]);
            }
        }
    }
}