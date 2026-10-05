using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Json;
using System.Diagnostics;
using Task_Management;
using Task_Management.Clients;
using Task_Management.Data;
using TaskManagement.Clients;

Trace.AutoFlush = true; 
Tracer.TaskManagerTrace.Switch.Level = SourceLevels.All;
Tracer.TaskManagerTrace.Listeners.Add(
    new TextWriterTraceListener("logs/taskmanagementTrace.log"));
Tracer.TaskManagerTrace.Flush();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(
        formatter: new JsonFormatter(),
        path: "logs/taskmanagementLogs.log",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("task_management")
    ?? throw new InvalidOperationException("Строка подключения 'task_management' не найдена.");

builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHttpClient<UserClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5135/");
});

builder.Services.AddHttpClient<ProjectClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5206/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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