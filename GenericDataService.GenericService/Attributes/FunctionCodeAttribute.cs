using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Attributes
{
    // Core/FunctionCodeAttribute.cs
    [AttributeUsage(AttributeTargets.Method)]
    public class FunctionCodeAttribute : Attribute
    {
        public string Code { get; }
        public FunctionCodeAttribute(string code) => Code = code;
    }
}
