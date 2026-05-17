using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardNTU.Models.Entities
{
    [Table("DuBaoRuiRo")]
    public class DuBaoRuiRo
    {
        [Key]
        [Column("id_prediction")]
        public int IdPrediction { get; set; }

        [Column("id_student")]
        public int IdStudent { get; set; }

        [Column("ma_lhp")]
        public string MaLhp { get; set; }

        [Column("xac_suat_truot")]
        public float XacSuatTruot { get; set; }

        [Column("nhan_du_bao")]
        public string NhanDuBao { get; set; }

        [Column("ly_do_chi_tiet")]
        public string LyDoChiTiet { get; set; }

        [Column("ngay_cap_nhat")]
        public DateTime NgayCapNhat { get; set; }

        [ForeignKey("IdStudent")]
        public virtual SinhVien SinhVien { get; set; }

        [ForeignKey("MaLhp")]
        public virtual LopHocPhan LopHocPhan { get; set; }
    }
}