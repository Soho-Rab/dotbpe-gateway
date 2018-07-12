using DotBPE.Protobuf;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DotBPE.AspNetGateway;
using Microsoft.Extensions.Hosting;
using DotBPE.Rpc.Hosting;

namespace SlideApp
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            this.Configuration = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加路由信息
            services.AddRoutes();

            //添加默认AspNetGateWay相关依赖
            services.AddSingleton<IMessageParser<AmpMessage>, MessageParser>();

            //添加服务端支持
            services.AddDotBPE();

            services.AddServiceActors<AmpMessage>((actors) =>
            {
                actors.Add<GreeterService>();
            });

            //添加RPC服务
            services.AddSingleton<IHostedService, RpcHostedService>();

            //添加消息转换器
            services.AddSingleton<IMessageParser<AmpMessage>, MessageParser>();
            services.AddSingleton<IProtobufDescriptorFactory, ProtobufDescriptorFactory>();
            //填写协议转换服务
            services.AddProtocolPipe<AmpMessage>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //使用网关
            app.UseGateway();
        }
    }
}