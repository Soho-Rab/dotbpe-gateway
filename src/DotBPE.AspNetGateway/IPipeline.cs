using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.AspNetGateway
{
    public interface IPipeline
    {
        Task<bool> Invoke(HttpContext context);
    }
}
