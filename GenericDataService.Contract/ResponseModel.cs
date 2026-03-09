using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.Contract
{
    // Core/ResponseModel.cs
    public class ResponseModel
    {
        public string Message { get; set; }
        public ResponseModel(Exception ex) => Message = ex.Message;
        public ResponseModel(string msg) => Message = msg;
    }
}
