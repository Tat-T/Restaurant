using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

// Добавление служб аутентификации
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
// Подключение аутентификации и авторизации
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();