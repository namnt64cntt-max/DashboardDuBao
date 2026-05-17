using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardNTU.Models.Entities
{
    [Table("HocKy")]
    public class HocKy
    {
        [Key]
        [Column("id_hoc_ky")]
        public int IdHocKy { get; set; }

        [Column("ten_hoc_ky")]
        public string TenHocKy { get; set; }

        [Column("nam_hoc")]
        public string NamHoc { get; set; }
    }
}