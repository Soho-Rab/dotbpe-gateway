using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.AspNetGateway
{
    public class PipeRuner
    {
        private readonly List<IPipeline> _pipelines;
        public PipeRuner(List<IPipeline> pipelines)
        {
            _pipelines = pipelines;
        }

        public async Task Invoke(HttpContext context)
        {           
            if (_pipelines.Count > 0)
            {
                foreach(var pp in _pipelines)
                {
                    var hasExc = await pp.Invoke(context);
                    if(hasExc)
                    {
                        break;
                    }
                }
            }           
        }
    }
}
