using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardNTU.Models.Entities
{
    [Table("SinhVien")]
    public class SinhVien
    {
        public SinhVien()
        {
            KetQuaHocTaps = new HashSet<KetQuaHocTap>();
        }

        [Key]
        [Column("id_student")]
        public int IdStudent { get; set; }

        [Column("ho_ten")]
        public string HoTen { get; set; }

        [Column("gioi_tinh")]
        public string GioiTinh { get; set; }

        [Column("que_quan")]
        public string QueQuan { get; set; }

        [Column("ma_lop")]
        public string MaLop { get; set; }

        [Column("bac_dao_tao")]
        public string BacDaoTao { get; set; }

        // Quan hệ Navigation
        public virtual ICollection<KetQuaHocTap> KetQuaHocTaps { get; set; }

        [ForeignKey("MaLop")]
        public virtual LopSinhVien LopSinhVien { get; set; }
    }
}