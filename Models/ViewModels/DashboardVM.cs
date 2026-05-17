namespace DashboardNTU.Models.ViewModels
{
    public class DashboardVM
    {
        public int TongSV { get; set; }
        public int SoSVDau { get; set; }
        public int SoSVRot { get; set; }
        public double GpaTrungBinh { get; set; }

        // Tự động tính tỷ lệ % ngay tại Model để View chỉ việc hiển thị
        public double PhanTramDau => TongSV > 0 ? (double)SoSVDau / TongSV * 100 : 0;
        public double PhanTramRot => TongSV > 0 ? (double)SoSVRot / TongSV * 100 : 0;

        // Dữ liệu cho biểu đồ Chart.js (Dùng mảng để dễ Serialize sang JavaScript)
        public string[] ChartLabels => new[] { "Đậu (%)", "Rớt (%)" };
        public double[] ChartData => new[] { PhanTramDau, PhanTramRot };
    }
}