using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardNTU.Models.Entities
{
    [Table("LopHocPhan")]
    public class LopHocPhan
    {
        [Key]
        [Column("ma_lhp")]
        public string MaLhp { get; set; }

        [Column("ma_mon")]
        public string MaMon { get; set; }

        [Column("id_hoc_ky")]
        public int IdHocKy { get; set; }

        [Column("giang_vien")]
        public string GiangVien { get; set; }

        [ForeignKey("MaMon")]
        public virtual MonHoc MonHoc { get; set; }

        [ForeignKey("IdHocKy")]
        public virtual HocKy HocKy { get; set; }
    }
}