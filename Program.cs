using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;
using MyRazorApp.Services;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IPasswordHasher<Users>, PasswordHasher<Users>>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Время жизни сессии
    options.Cookie.HttpOnly = true; // Защита от доступа через JS
    options.Cookie.IsEssential = true; // Сессионный куки обязателен
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Время жизни куки
        options.SlidingExpiration = true; // Продление куки при активности
    });

builder.Services.AddAuthorization();
builder.Services.AddRazorPages();


var app = builder.Build();

var cultureInfo = new CultureInfo("ru-RU");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.UseRouting();
app.UseStaticFiles();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ---- Вызов миграции паролей ----
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Users>>();

    await PasswordMigration.RunAsync(context, passwordHasher);
}
// --------------------------------

app.MapRazorPages();

app.Run();