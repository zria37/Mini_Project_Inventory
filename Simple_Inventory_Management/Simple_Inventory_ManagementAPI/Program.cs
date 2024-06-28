using Hangfire;
using Microsoft.EntityFrameworkCore;
using Simple_Inventory_ManagementAPI.Dal;
using Simple_Inventory_ManagementAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Ini untuk ketika running body request tidak berpengaruh besar kecil teksnya
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// Add services to the container
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Access dulu db contextnya
builder.Services.AddDbContext<InventoryDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HangfireDB")));

// Config db
var hangfireDb = builder.Configuration.GetConnectionString("HangfireDB");

// Dependency injection menggunakan EF
builder.Services.AddScoped<ITransaction, Transactions>();

// Hangfire service
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseDefaultTypeSerializer()
    .UseSqlServerStorage(hangfireDb, new Hangfire.SqlServer.SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    })
);
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHangfireDashboard();
app.UseAuthorization();

app.MapControllers();

app.Run();
