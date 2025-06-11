using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Repository;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logger Configuration
var logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
Directory.CreateDirectory(logDirectory); // Ensure Directory Exists

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

string databaseConnectionString = builder.Configuration.GetConnectionString(name: "DatabaseConnection")!;

// Database Configuration
builder.Services.AddDbContext<NewsAggregationDbContext>(optionsAction => optionsAction.UseSqlServer(databaseConnectionString));
builder.Services.AddScoped<DbContext, NewsAggregationDbContext>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(Program));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped(typeof(ICrudBaseRepository<,>), typeof(CrudBaseRepository<,>));
builder.Services.AddScoped(typeof(ICrudBaseService<,>), typeof(CrudBaseService<,>));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
