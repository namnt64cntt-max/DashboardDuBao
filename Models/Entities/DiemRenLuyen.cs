using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardNTU.Models.Entities
{
    [Table("DiemRenLuyen")]
    public class DiemRenLuyen
    {
        [Key, Column("id_student", Order = 0)]
        public int IdStudent { get; set; }

        [Key, Column("id_hoc_ky", Order = 1)]
        public int IdHocKy { get; set; }

        [Column("diem_rl")]
        public int DiemRl { get; set; }

        [ForeignKey("IdStudent")]
        public virtual SinhVien SinhVien { get; set; }

        [ForeignKey("IdHocKy")]
        public virtual HocKy HocKy { get; set; }
    }
}