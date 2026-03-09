using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Models
{
    public class ServiceContext
    {
        public string UserId { get; set; } = default!;
        public string Signature { get; set; } = default!;
        public string FunctionCode { get; set; } = default!;
        public JsonElement Payload { get; set; }
        public ServicePermission Permission { get; set; } = default!;

        // Written by DbExecution, read by post-steps
        public ServiceResult? Result { get; set; }

        // Shared bag for anything steps need to pass forward
        // e.g. AuditStep needs the "before" snapshot taken pre-execution
        public Dictionary<string, object> Bag { get; set; } = new();
    }
}
