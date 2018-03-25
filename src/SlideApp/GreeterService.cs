using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlideApp
{
    public class GreeterService : GreeterServiceBase
    {
        public override Task<RpcResult<HelloRes>> SayHelloAgainAsync(HelloReq req)
        {
            RpcResult<HelloRes> res = new RpcResult<HelloRes>();
            res.Data = new HelloRes()
            {
                GreetWord = string.Format("Hello {0} Again,From {1}", req.Name, req.ClientIp)
            };

            return Task.FromResult(res);
        }

        public override Task<RpcResult<HelloRes>> SayHelloAsync(HelloReq req)
        {
            RpcResult<HelloRes> res = new RpcResult<HelloRes>();
            res.Data = new HelloRes()
            {
                GreetWord = string.Format("Hello {0} ,From {1}", req.Name, req.ClientIp)
            };

            return Task.FromResult(res);
        }
    }
}
