using Microsoft.EntityFrameworkCore;
using DashboardNTU.Data; // ??m b?o ?úng namespace folder Data

var builder = WebApplication.CreateBuilder(args);

// 1. L?y chu?i k?t n?i t? file appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. C?u hình Entity Framework Core k?t n?i v?i MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 3. Thêm d?ch v? cho MVC (Controllers và Views)
builder.Services.AddControllersWithViews();

// (Tùy ch?n) ??ng ký các Service x? lý logic n?u b?n có
// builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

var app = builder.Build();

// 4. C?u hình HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Cho phép dùng CSS, JS, hình ?nh trong wwwroot

app.UseRouting();

app.UseAuthorization();

// 5. C?u hình Route m?c ??nh (Ch?y Dashboard ngay khi m? web)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();