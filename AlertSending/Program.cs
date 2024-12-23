using AlertSending.Classes;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddHangfire(options =>
//{
//    options.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("Conn"));
//});
//builder.Services.AddHangfireServer();
var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
if (!Directory.Exists(logDirectory))
{
    Directory.CreateDirectory(logDirectory);
}
// Enable self-logging
Serilog.Debugging.SelfLog.Enable(msg => System.Diagnostics.Debug.WriteLine(msg));
Serilog.Debugging.SelfLog.Enable(Console.Error);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .WriteTo.Async(a => a.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Level == LogEventLevel.Information)
        .WriteTo.File(Path.Combine(logDirectory, "information-.txt"), rollingInterval: RollingInterval.Hour, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")))
    .WriteTo.Async(a => a.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Level == LogEventLevel.Warning)
        .WriteTo.File(Path.Combine(logDirectory, "warnings-.txt"), rollingInterval: RollingInterval.Hour, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")))
    .WriteTo.Async(a => a.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Level == LogEventLevel.Error)
        .WriteTo.File(Path.Combine(logDirectory, "errors-.txt"), rollingInterval: RollingInterval.Hour, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")))
    .CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<CommonConfigurations>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<CommonConfigurations>();
builder.Services.AddTransient<EmailSender>();
builder.Services.AddTransient<SmsSender>();
builder.Services.AddWindowsService();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var Port = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Port"];
    serverOptions.ListenAnyIP(Convert.ToInt16(Port)); // Change the port here
});
var app = builder.Build();
var commonConfig = app.Services.GetRequiredService<CommonConfigurations>();
commonConfig.LoadConfigurations();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHangfireDashboard("/hangfire");
app.UseRouting();
app.UseStaticFiles();   
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
