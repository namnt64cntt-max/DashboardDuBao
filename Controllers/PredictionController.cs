using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DashboardNTU.Data;
using DashboardNTU.Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace DashboardNTU.Controllers
{
    public class PredictionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PredictionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách dự báo
        public async Task<IActionResult> List()
        {
            // Lấy 1000 ca Nguy cơ và 1000 ca An toàn để demo nhanh
            var highRisk = await _context.DuBaoRuiRos
                .OrderByDescending(p => p.XacSuatTruot)
                .Take(1000)
                .ToListAsync();

            var lowRisk = await _context.DuBaoRuiRos
                .OrderBy(p => p.XacSuatTruot)
                .Take(1000)
                .ToListAsync();

            var result = highRisk.Concat(lowRisk).OrderBy(x => Guid.NewGuid()).ToList();
            return View(result);
        }

        // Kích hoạt script Python dự báo
        [HttpPost]
        public IActionResult RunAIPrediction()
        {
            try
            {
                // Lấy đường dẫn tuyệt đối đến file Python trong thư mục dự án
                string projectRoot = Directory.GetCurrentDirectory();
                string scriptPath = Path.Combine(projectRoot, "AI_Module", "predict_risk.py");

                ProcessStartInfo start = new ProcessStartInfo();
                // Nếu máy Nam dùng lệnh 'python' thì đổi thành "python"
                start.FileName = "py";
                start.Arguments = scriptPath;
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.CreateNoWindow = true;

                using (Process process = Process.Start(start))
                {
                    // Chờ script chạy xong (quan trọng vì dữ liệu lớn)
                    process.WaitForExit();
                    return Json(new { success = true, message = "AI đã hoàn thành phân tích 115k bản ghi và cập nhật Database!" });
                }
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = "Lỗi thực thi: " + ex.Message });
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            var prediction = await _context.DuBaoRuiRos
                .FirstOrDefaultAsync(m => m.IdPrediction == id);
            if (prediction == null) return NotFound();
            return View(prediction);
        }
    }
}