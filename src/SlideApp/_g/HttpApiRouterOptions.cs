// Generated by the protocol buffer compiler.  DO NOT EDIT!
#region Designer generated code

using DotBPE.Protobuf;
using System.Collections.Generic;

namespace SlideApp {
    public static class HttpApiRouterOptions {
        public static List<HttpApiOption> GetList()
        {
            var list = new List<HttpApiOption>();

            list.Add(new HttpApiOption()
            {
                ServiceId = 10000,
                MessageId = 1,
                Path = "/api/greeter/sayhello",
                Method = "get",
                Description ="SayHello 服务",
                Timeout = 0,
                Plugin = ""
            });

            list.Add(new HttpApiOption()
            {
                ServiceId = 10000,
                MessageId = 2,
                Path = "/api/greeter/sayhelloagain",
                Method = "get",
                Description ="SayHelloAgain 服务",
                Timeout = 0,
                Plugin = ""
            });
 return list;
        }
    }
}
#endregion
