using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Models
{
    public class ConnectionRegistry
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public DateTime ConnectionTime { get; set; }
    }
}
