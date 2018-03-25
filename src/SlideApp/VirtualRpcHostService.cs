using System;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SlideApp
{
    public class VirtualRpcHostService : RpcHostedService, IHostedService
    {
        public VirtualRpcHostService(IServiceProvider hostProvider,
            IConfiguration config,
            ILogger<RpcHostedService> logger,
            ILoggerFactory loggerFactory) : base(hostProvider, config, logger, loggerFactory)
        {
        }
    }
}