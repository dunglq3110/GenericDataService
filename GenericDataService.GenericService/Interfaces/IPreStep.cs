using GenericDataService.GenericService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Interfaces
{
    public interface IPreStep
    {
        Task<ServiceResult?> ExecuteAsync(ServiceContext ctx, Func<Task<ServiceResult?>> next);
    }
}
