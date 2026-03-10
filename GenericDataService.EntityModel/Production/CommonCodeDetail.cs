using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GenericDataService.EntityModel.Production
{
    [Table("CommonCodeDetails", Schema = "System")]
    public class CommonCodeDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = default!;

        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = default!;

        [MaxLength(200)]
        public string? Value1 { get; set; }

        [MaxLength(200)]
        public string? Value2 { get; set; }

        public int MasterId { get; set; }

        [ForeignKey(nameof(MasterId))]
        [JsonIgnore]
        public CommonCodeMaster Master { get; set; } = default!;
    }
}
