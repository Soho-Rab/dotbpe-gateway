using DotBPE.Rpc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.AspNetGateway
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProtocolPipe<TMessage>(this IServiceCollection services) where TMessage:InvokeMessage
        {
            return services.AddSingleton<IPipeline, Pipelines.ProtocolPipe<TMessage>>();
        }
    }
}
