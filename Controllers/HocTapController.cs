using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DashboardNTU.Data;
using DashboardNTU.Models.Entities;
using System.Linq;

namespace DashboardNTU.Controllers
{
    public class HocTapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HocTapController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            // 1. LẤY DỮ LIỆU TOP 5 MÔN HỌC RỦI RO NHẤT (Để vẽ biểu đồ cột)
            var riskSubjects = await _context.KetQuaHocTaps
                .Include(k => k.LopHocPhan).ThenInclude(l => l.MonHoc)
                .Where(k => k.TrangThaiThucTe == "Rớt")
                .GroupBy(k => new { k.LopHocPhan.MonHoc.TenMon, k.LopHocPhan.GiangVien })
                .Select(g => new {
                    Label = g.Key.TenMon + " (" + g.Key.GiangVien + ")",
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            ViewBag.RiskLabels = riskSubjects.Select(x => x.Label).ToList();
            ViewBag.RiskValues = riskSubjects.Select(x => x.Count).ToList();

            // 2. QUERY DỮ LIỆU BẢNG ĐIỂM CHI TIẾT
            var query = _context.KetQuaHocTaps
                .Include(k => k.SinhVien)
                .Include(k => k.LopHocPhan).ThenInclude(l => l.MonHoc)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(k => k.IdStudent.ToString() == searchString);
                ViewBag.ChartTitle = $"Tiến độ học tập sinh viên: {searchString}";
            }
            else
            {
                ViewBag.ChartTitle = "Xu hướng GPA Trung bình toàn trường";
            }

            // 3. TÍNH TOÁN DỮ LIỆU BIỂU ĐỒ XU HƯỚNG
            var trendQuery = _context.KetQuaHocTaps.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                trendQuery = trendQuery.Where(k => k.IdStudent.ToString() == searchString);
            }

            var trendData = await trendQuery
                .Include(k => k.LopHocPhan).ThenInclude(l => l.HocKy)
                .GroupBy(k => new { k.LopHocPhan.HocKy.TenHocKy, k.LopHocPhan.HocKy.NamHoc, k.LopHocPhan.IdHocKy })
                .Select(g => new {
                    Ky = g.Key.TenHocKy + " " + g.Key.NamHoc,
                    Gpa = g.Average(x => x.DiemTongKet),
                    SortId = g.Key.IdHocKy
                })
                .OrderBy(x => x.SortId).ToListAsync();

            ViewBag.TrendLabels = trendData.Select(x => x.Ky).ToList();
            ViewBag.TrendValues = trendData.Select(x => x.Gpa).ToList();

            return View(await query.OrderBy(k => k.IdStudent).Take(50).ToListAsync());
        }
    }
}