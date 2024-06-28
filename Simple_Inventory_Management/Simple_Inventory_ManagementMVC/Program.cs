using Microsoft.EntityFrameworkCore;
using Simple_Inventory_ManagementMVC.Dal;
using Simple_Inventory_ManagementMVC.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<InventoryDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add HttpClientFactory
builder.Services.AddHttpClient();

builder.Services.AddScoped<IProduct, Products>();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
