using Microsoft.EntityFrameworkCore;
using NekoFood.Models;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// ³]©w NLog
builder.Host.UseNLog();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<NekoFoodContext>(options => options.UseSqlServer("Name=ConnectionStrings:DefaultConnection"));
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession(new SessionOptions()
{
    Cookie = new CookieBuilder()
    {
        Name = ".AspNetCore.Session.NekoFood"
    }
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
