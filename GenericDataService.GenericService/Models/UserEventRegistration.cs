using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService.Models
{
    public class UserEventRegistration
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Events { get; set; } = "[]"; // JSON array stored as string
    }
}
