using GenericDataService.GenericService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Interfaces
{
    // Core/IGenericService.cs
    public interface IGenericService
    {
        Task<ServiceResult> HandleAsync(string functionCode, JsonElement payload);
    }
}
