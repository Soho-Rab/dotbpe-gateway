// Generated by the protocol buffer compiler. DO NOT EDIT!
// source: greeterService_10000.proto
#region Designer generated code

using System;
using System.Threading.Tasks;
using DotBPE.Rpc;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc.Exceptions;
using Google.Protobuf;
using DotBPE.Rpc.Client;

namespace SlideApp {

    //start for class GreeterServiceClient
    public sealed class GreeterServiceClient : AmpInvokeClient
    {
        public GreeterServiceClient(ICallInvoker<AmpMessage> callInvoker) : base(callInvoker)
        {

        }

        //同步方法
        public RpcResult<HelloRes> SayHello(HelloReq req)
        {
            AmpMessage message = AmpMessage.CreateRequestMessage(10000, 1);

            message.FriendlyServiceName = "GreeterService.SayHello";


            message.Data = req.ToByteArray();
            var response = base.CallInvoker.BlockingCall(message);
            if (response == null)
            {
                throw new RpcException("error,response is null !");
            }
            var result = new RpcResult<HelloRes>();
            if (response.Code != 0)
            {
                result.Code = response.Code;
            }
            else if (response.Data == null)
            {
                result.Code = ErrorCodes.CODE_INTERNAL_ERROR;
            }
            else
            {
                result.Data = HelloRes.Parser.ParseFrom(response.Data);
            }
            return result;
        }

        public async Task<RpcResult<HelloRes>> SayHelloAsync(HelloReq req, int timeOut = 3000)
        {
            AmpMessage message = AmpMessage.CreateRequestMessage(10000, 1);
            message.FriendlyServiceName = "GreeterService.SayHello";
            message.Data = req.ToByteArray();
            var response = await base.CallInvoker.AsyncCall(message, timeOut);
            if (response == null)
            {
                throw new RpcException("error,response is null !");
            }
           var result = new RpcResult<HelloRes>();
            if (response.Code != 0)
            {
                result.Code = response.Code;
            }
            else if (response.Data == null)
            {
                result.Code = ErrorCodes.CODE_INTERNAL_ERROR;
            }
            else
            {
                result.Data = HelloRes.Parser.ParseFrom(response.Data);
            }

            return result;
        }

        //同步方法
        public RpcResult<HelloRes> SayHelloAgain(HelloReq req)
        {
            AmpMessage message = AmpMessage.CreateRequestMessage(10000, 2);

            message.FriendlyServiceName = "GreeterService.SayHelloAgain";


            message.Data = req.ToByteArray();
            var response = base.CallInvoker.BlockingCall(message);
            if (response == null)
            {
                throw new RpcException("error,response is null !");
            }
            var result = new RpcResult<HelloRes>();
            if (response.Code != 0)
            {
                result.Code = response.Code;
            }
            else if (response.Data == null)
            {
                result.Code = ErrorCodes.CODE_INTERNAL_ERROR;
            }
            else
            {
                result.Data = HelloRes.Parser.ParseFrom(response.Data);
            }
            return result;
        }

        public async Task<RpcResult<HelloRes>> SayHelloAgainAsync(HelloReq req, int timeOut = 3000)
        {
            AmpMessage message = AmpMessage.CreateRequestMessage(10000, 2);
            message.FriendlyServiceName = "GreeterService.SayHelloAgain";
            message.Data = req.ToByteArray();
            var response = await base.CallInvoker.AsyncCall(message, timeOut);
            if (response == null)
            {
                throw new RpcException("error,response is null !");
            }
           var result = new RpcResult<HelloRes>();
            if (response.Code != 0)
            {
                result.Code = response.Code;
            }
            else if (response.Data == null)
            {
                result.Code = ErrorCodes.CODE_INTERNAL_ERROR;
            }
            else
            {
                result.Data = HelloRes.Parser.ParseFrom(response.Data);
            }

            return result;
        }

    }
    //end for class GreeterServiceClient
}
#endregion