using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DashboardNTU.Data;
using DashboardNTU.Models.Entities;

namespace DashboardNTU.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SinhVienController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index(string maLop)
        {
            ViewBag.LopList = await _context.LopSinhViens.AsNoTracking().ToListAsync();
            var query = _context.SinhViens
                .Include(s => s.LopSinhVien).ThenInclude(l => l.ChuyenNganh)
                .Include(s => s.KetQuaHocTaps).ThenInclude(k => k.LopHocPhan).ThenInclude(l => l.HocKy)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(maLop)) query = query.Where(s => s.MaLop == maLop);

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var sinhVien = await _context.SinhViens
                .Include(s => s.LopSinhVien) // Quan trọng: Lấy thông tin lớp để hiển thị khóa học
                .Include(s => s.KetQuaHocTaps)
                    .ThenInclude(k => k.LopHocPhan)
                        .ThenInclude(l => l.MonHoc)
                .Include(s => s.KetQuaHocTaps)
                    .ThenInclude(k => k.LopHocPhan)
                        .ThenInclude(l => l.HocKy)
                .FirstOrDefaultAsync(s => s.IdStudent == id);

            if (sinhVien == null) return NotFound();

            // TÍNH TỔNG TÍN CHỈ: Chỉ tính những môn có điểm tổng kết >= 5 (Đậu)
            ViewBag.TongTinChi = sinhVien.KetQuaHocTaps
                .Where(k => k.DiemTongKet >= 5)
                .Sum(k => k.LopHocPhan?.MonHoc?.SoTinChi ?? 0);

            // LẤY ĐIỂM RÈN LUYỆN
            ViewBag.DiemRenLuyen = await _context.DiemRenLuyens
                .Where(d => d.IdStudent == id)
                .OrderBy(d => d.IdHocKy)
                .ToListAsync();

            return View(sinhVien);
        }
    }
}