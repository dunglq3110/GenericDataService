using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Models
{
    // Core/ServiceResult.cs
    public class ServiceResult
    {
        public bool Success { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }

        public static ServiceResult Ok(object? data = null) =>
            new() { Success = true, Data = data };

        public static ServiceResult Fail(string message) =>
            new() { Success = false, Message = message };
    }
}
