using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceSignatureAttribute : Attribute
    {
        public string Signature { get; }

        public ServiceSignatureAttribute(string signature)
        {
            Signature = signature;
        }
    }
}
