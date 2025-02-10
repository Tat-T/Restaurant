using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MyRazorApp.Pages.Admin;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSession();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSession();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!db.AdminUsers.Any())
    {
        db.AdminUsers.Add(new AdminUser
        {
            Username = "admin",
            PasswordHash = ComputeSha256Hash("12345") // Вынесем метод отдельно
        });
        db.SaveChanges();
    }
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();

// Метод для хеширования паролей
static string ComputeSha256Hash(string rawData)
{
    using (System.Security.Cryptography.SHA256 sha256Hash = System.Security.Cryptography.SHA256.Create())
    {
        byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        foreach (byte t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }
}
