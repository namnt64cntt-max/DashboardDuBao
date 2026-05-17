using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardNTU.Models.Entities
{
    [Table("LopSinhVien")]
    public class LopSinhVien
    {
        [Key]
        [Column("ma_lop")]
        public string MaLop { get; set; }

        [Column("ten_lop")]
        public string TenLop { get; set; }

        [Column("khoa_hoc")]
        public int? KhoaHoc { get; set; }

        [Column("ma_nganh")]
        public string MaNganh { get; set; }

        [ForeignKey("MaNganh")]
        public virtual ChuyenNganh ChuyenNganh { get; set; }
    }
}