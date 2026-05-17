using Microsoft.EntityFrameworkCore;
using DashboardNTU.Models.Entities;

namespace DashboardNTU.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ChuyenNganh> ChuyenNganhs { get; set; }
        public DbSet<LopSinhVien> LopSinhViens { get; set; }
        public DbSet<SinhVien> SinhViens { get; set; }
        public DbSet<HocKy> HocKys { get; set; }
        public DbSet<DiemRenLuyen> DiemRenLuyens { get; set; }
        public DbSet<MonHoc> MonHocs { get; set; }
        public DbSet<LopHocPhan> LopHocPhans { get; set; }
        public DbSet<KetQuaHocTap> KetQuaHocTaps { get; set; }
        public DbSet<DuBaoRuiRo> DuBaoRuiRos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình Composite Key cho DiemRenLuyen
            modelBuilder.Entity<DiemRenLuyen>()
                .HasKey(d => new { d.IdStudent, d.IdHocKy });

            // Cấu hình Composite Key cho KetQuaHocTap
            modelBuilder.Entity<KetQuaHocTap>()
                .HasKey(k => new { k.IdStudent, k.MaLhp });

            base.OnModelCreating(modelBuilder);
        }
    }
}