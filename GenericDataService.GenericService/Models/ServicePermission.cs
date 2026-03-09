using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Models
{
    // Infrastructure/ServicePermission.cs
    // Table ServicePermissions
    public class ServicePermission
    {
        //primary key, auto increment
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public string Signature { get; set; } = default!;
        public string Action { get; set; } = default!;
        public bool IsAllow { get; set; }
        public bool IsValidation { get; set; }
        public string? SpName { get; set; }
        public string? AllowedParams { get; set; }      // JSON array
        public string? ValidationOptions { get; set; }  // JSON object

        public bool IsNotify { get; set; }
        public string? Events { get; set; }  // JSON object

    }
}
