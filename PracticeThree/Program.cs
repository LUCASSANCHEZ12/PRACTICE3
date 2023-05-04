using Microsoft.OpenApi.Models;
using Serilog;
using UPB.CoreLogic.Managers;
using UPB.PracticeThree.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var configurationBuilder = new ConfigurationBuilder()
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

IConfiguration Configuration = configurationBuilder.Build();
string siteTitle = Configuration.GetSection("Title").Value;
string textPath = Configuration.GetSection("PatientFile").Value;
string logPath = Configuration.GetSection("LogFile").Value;

//create the logger and setup your sinks, filters and properties
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(Configuration)
    .CreateLogger();

Log.Information("Path patients file: "+textPath);
Log.Information("Path log file: "+logPath);
builder.Services.AddTransient<PatientManager>(ServiceProvider => new PatientManager(textPath));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = siteTitle
    });
});

var app = builder.Build();

app.UseGlobalExceptionHandler(Log.Logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsEnvironment(builder.Environment.EnvironmentName))
{
    Log.Information("Ejecutando en el ambiente de " + builder.Environment.EnvironmentName);
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();