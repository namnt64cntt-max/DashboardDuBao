using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardNTU.Models.Entities
{
    [Table("KetQuaHocTap")]
    public class KetQuaHocTap
    {
        [Column("id_student")]
        public int IdStudent { get; set; }

        [Column("ma_lhp")]
        public string MaLhp { get; set; }

        [Column("diem_chuyen_can")]
        public float DiemChuyenCan { get; set; }

        [Column("diem_giua_ky")]
        public float DiemGiuaKy { get; set; }

        [Column("diem_cuoi_ky")]
        public float DiemCuoiKy { get; set; }

        [Column("diem_tong_ket")]
        public float DiemTongKet { get; set; }

        [Column("trang_thai_thuc_te")]
        public string TrangThaiThucTe { get; set; }

        [ForeignKey("IdStudent")]
        public virtual SinhVien SinhVien { get; set; }

        [ForeignKey("MaLhp")]
        public virtual LopHocPhan LopHocPhan { get; set; }


    }
}