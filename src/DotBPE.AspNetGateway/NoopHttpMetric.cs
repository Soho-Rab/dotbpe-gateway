using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotBPE.AspNetGateway
{
    public class NoopHttpMetric : IHttpMetric
    {
        public Task AddToMetricsAsync(HttpRequest req, HttpResponse res)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
