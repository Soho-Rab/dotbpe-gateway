// Generated by the protocol buffer compiler.  DO NOT EDIT!
#region Designer generated code

using DotBPE.Protobuf;
using System.Collections.Generic;

namespace SlideApp {
    public static class HttpApiRouterOptions {


        static Dictionary<string, List<HttpApiOption>> routeDict = new Dictionary<string, List<HttpApiOption>>();
        static HttpApiRouterOptions()
        { 
            AddRouter("",new HttpApiOption()
            {
                ServiceId = 10000,
                MessageId = 1,
                Path = "/api/greeter/sayhello",
                Method = "get",
                Description ="SayHello 服务",
                Timeout = 0,
                Plugin = ""
            });
            AddRouter("",new HttpApiOption()
            {
                ServiceId = 10000,
                MessageId = 2,
                Path = "/api/greeter/sayhelloagain",
                Method = "get",
                Description ="SayHelloAgain 服务",
                Timeout = 0,
                Plugin = ""
            }); 
        }

        private static void AddRouter(string category, HttpApiOption routerOption)
        {
            category = string.IsNullOrEmpty(category) ? "default" : category;
            if (!routeDict.ContainsKey(category))
            {
                routeDict.Add(category, new List<HttpApiOption>());
            }          
            routeDict[category].Add(routerOption);
        }

        public static List<HttpApiOption> GetList(string category="default")
        {
            if (routeDict.ContainsKey(category))
            {
                return routeDict[category];
            }

            return new List<HttpApiOption>();
        }
    }
}
#endregion
