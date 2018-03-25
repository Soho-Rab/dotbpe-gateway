using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.AspNetGateway
{
    public class PluginFactory
    {

        /// <summary>
        /// 创建插件
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static IHttpPlugin CreatePlugin(string typeName,IServiceProvider provider)
        {
            return (IHttpPlugin)ActivatorUtilities.GetServiceOrCreateInstance(provider, Type.GetType(typeName));
        }

        //创建插件
        public static IHttpPlugin CreatePlugin(string typeName)
        {
            Rpc.Utils.Assert.IsNotNull(Rpc.Environment.ServiceProvider, "服务未启动前，不能调用");
            return (IHttpPlugin)ActivatorUtilities.GetServiceOrCreateInstance(Rpc.Environment.ServiceProvider, Type.GetType(typeName));
        }
    }
}
