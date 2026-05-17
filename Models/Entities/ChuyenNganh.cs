using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardNTU.Models.Entities
{
    [Table("ChuyenNganh")]
    public class ChuyenNganh
    {
        [Key]
        [Column("ma_nganh")]
        public string MaNganh { get; set; }

        [Column("ten_nganh")]
        public string TenNganh { get; set; }
    }
}