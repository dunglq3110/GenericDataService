using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Models
{
    public class ValidationOptions
    {
        public List<ValidationRule> Rules { get; set; } = new();
    }

    public class ValidationRule
    {
        public string Field { get; set; } = default!;
        public bool? Required { get; set; }
        public int? MaxLength { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public string? Regex { get; set; }
        public string? Type { get; set; } // "int", "string", etc. — extend as needed
    }
}
