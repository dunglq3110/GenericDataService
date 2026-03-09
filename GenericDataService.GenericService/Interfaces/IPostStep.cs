using GenericDataService.GenericService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Interfaces
{
    public interface IPostStep
    {
        // Result is already in ctx.Result — no next(), just side effects
        Task ExecuteAsync(ServiceContext ctx);
    }
}
