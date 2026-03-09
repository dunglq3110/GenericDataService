using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Interfaces
{
    public interface IServiceFactory
    {
        IGenericService GetService(string signature);
    }
}
