using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Repository;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Authentication Configuration using JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
string issuer = jwtSettings["Issuer"]!;
string audience = jwtSettings["Audience"]!;
string secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

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

//TODO: Apply Filter/Sorting in GET ALL API's
//TODO: Check Authorize Roles above controller
//TODO: Fix UserId1 Column in DB
builder.Services.AddScoped(typeof(ICrudBaseRepository<,>), typeof(CrudBaseRepository<,>));
builder.Services.AddScoped(typeof(ICrudBaseService<,>), typeof(CrudBaseService<,>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserKeywordService, UserKeywordService>();
builder.Services.AddScoped<IExternalServerService, ExternalServerService>();
builder.Services.AddScoped<IUserNotificationConfigurationService, UserNotificationConfigurationService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IUserNotificationService, UserNotificationService>();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT",
//        Description = "JWT Authorization header using the Bearer scheme."
//    });
//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            new string[] { }
//        }
//    });
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
