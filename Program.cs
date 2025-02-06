using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

void SeedDatabase(AppDbContext context)
{
    if (!context.Admins.Any())
    {
        using var sha256 = SHA256.Create();
        var passwordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes("admin123"));
        var hashString = BitConverter.ToString(passwordHash).Replace("-", "").ToLower();

        context.Admins.Add(new Admin { Username = "admin", PasswordHash = hashString });
        context.SaveChanges();
    }
}


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
    SeedDatabase(dbContext);
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
