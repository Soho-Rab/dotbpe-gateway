using DotBPE.Rpc;
using DotBPE.Rpc.Codes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.AspNetGateway
{
    public interface IHttpPlugin
    {

    }


    public interface IHttpPreProcessPlugin : IHttpPlugin
    {
        Task<bool> PreProcessAsync(HttpRequest req,HttpResponse res, HttpRouterOptionItem routeOption);
    }


    public interface IHttpParsePlugin: IHttpPlugin
    {
        Task<bool> ParseAsync(HttpRequest req, HttpResponse res, RequestData dictData, HttpRouterOptionItem routeOption);
    }

    public interface IHttpPostParsePlugin : IHttpPlugin
    {
        Task<bool> PostParseAsync(HttpRequest req, HttpResponse res, RequestData dictData, HttpRouterOptionItem routeOption);
    }

    /// <summary>
    /// 用于处理HTTP响应发送前的信息
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <seealso cref="DotBPE.Gateway.Amp.IHttpPlugin" />
    public interface IHttpPostProcessPlugin<TMessage> : IHttpPlugin where TMessage : InvokeMessage
    {
        Task<bool> PostProcessAsync(HttpRequest req, HttpResponse res,TMessage msg, HttpRouterOptionItem routeOption);
    }

    public interface IHttpOutputPlugin<TMessage> : IHttpPlugin where TMessage : InvokeMessage
    {
        Task<bool> OutputAsync(HttpRequest req, HttpResponse res, TMessage msg, HttpRouterOptionItem routeOption);
    }
}
