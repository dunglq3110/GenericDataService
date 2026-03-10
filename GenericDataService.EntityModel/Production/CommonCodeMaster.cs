using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.EntityModel.Production
{
    [Table("CommonCodeMasters", Schema = "System")]
    public class CommonCodeMaster
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = default!;

        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = default!;

        public ICollection<CommonCodeDetail>? Details { get; set; }
    }
}
