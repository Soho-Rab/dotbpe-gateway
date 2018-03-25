using DotBPE.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.AspNetGateway
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseGateway(this IApplicationBuilder app) 
        {
            var provider = app.ApplicationServices;
            //构建路由配置中的类型
            var options = provider.GetRequiredService<IOptions<HttpRouterOption>>();
            if (options != null && options.Value != null)
            {
                foreach (var router in options.Value.Items)
                {
                    if (!string.IsNullOrEmpty(router.PluginName))
                    {
                        router.Plugin = PluginFactory.CreatePlugin(router.PluginName, provider);
                    }
                }
            }
            var pipelines = provider.GetServices<IPipeline>();
            var pplist = pipelines.ToList();
            var runer = new PipeRuner(pplist);

            app.Run(runer.Invoke);
          
            return app;
        }

    }
}
