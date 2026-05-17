using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DashboardNTU.Data;
using DashboardNTU.Models.Entities;
using DashboardNTU.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

namespace DashboardNTU.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? page)
        {
            // --- 1. THỐNG KÊ KPI ---
            var stats = new DashboardVM();

            // Tổng số sinh viên thực tế
            stats.TongSV = await _context.SinhViens.CountAsync();

            // Số sinh viên từng rớt ít nhất 1 môn
            var listSvTungRot = await _context.KetQuaHocTaps
                .Where(k => k.TrangThaiThucTe == "Rớt")
                .Select(k => k.IdStudent)
                .Distinct()
                .ToListAsync();

            stats.SoSVRot = listSvTungRot.Count;
            stats.SoSVDau = stats.TongSV - stats.SoSVRot;

            // Tính GPA Trung Bình
            if (await _context.KetQuaHocTaps.AnyAsync())
            {
                stats.GpaTrungBinh = await _context.KetQuaHocTaps.AverageAsync(k => (double)k.DiemTongKet);
            }

            // Gán biến stats vào ViewBag để truyền qua View
            ViewBag.Stats = stats;

            // --- 2. LẤY DỮ LIỆU BẢNG & PHÂN TRANG (Giữ nguyên logic của bạn) ---
            var query = _context.KetQuaHocTaps
                .Include(k => k.SinhVien)
                .Include(k => k.LopHocPhan).ThenInclude(l => l.MonHoc)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(k => k.IdStudent.ToString().Contains(searchString) ||
                                    k.SinhVien.HoTen.Contains(searchString));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(k => k.IdStudent)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var pagedData = new StaticPagedList<KetQuaHocTap>(items, pageNumber, pageSize, totalCount);
            return View(pagedData);
        }
    }
}