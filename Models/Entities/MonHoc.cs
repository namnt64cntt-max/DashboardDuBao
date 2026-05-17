using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardNTU.Models.Entities
{
    [Table("MonHoc")]
    public class MonHoc
    {
        [Key]
        [Column("ma_mon")]
        public string MaMon { get; set; }

        [Column("ten_mon")]
        public string TenMon { get; set; }

        [Column("so_tin_chi")]
        public int SoTinChi { get; set; }
    }
}