using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewsAggregation.Configurations.DatabaseConfigurations;
using Polly;
using NewsAggregation.ExternalServers.Factory.Contracts;
using NewsAggregation.ExternalServers.Factory;
using NewsAggregation.ExternalServers.Services.Contracts;
using NewsAggregation.Repository;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using Serilog;
using System.Text;
using NewsAggregation.Models;
using NewsAggregation.Notifications.Contracts;
using NewsAggregation.Notifications;
using Hangfire;
using NewsAggregation.ExternalServers.Services;
using NewsAggregation.Middlewares;


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


builder.Services.AddHttpClient();
builder.Services.AddControllers();

//TODO: Add services to the container.
//TODO: Apply Filter/Sorting in GET ALL API's
//TODO: Check Authorize Roles above controller
builder.Services.AddScoped(typeof(ICrudBaseService<>), typeof(CrudBaseService<>));
builder.Services.AddScoped(typeof(ICrudBaseRepository<>), typeof(CrudBaseRepository<>));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IKeywordService, KeywordService>();
builder.Services.AddScoped<IUserKeywordService, UserKeywordService>();

builder.Services.AddScoped<IExternalServerService, ExternalServerService>();
builder.Services.AddScoped<IExternalServerRepository, ExternalServerRepository>();

builder.Services.AddScoped<IUserNotificationConfigurationService, UserNotificationConfigurationService>();
builder.Services.AddScoped<IUserNotificationConfigurationRepository, UserNotificationConfigurationRepository>();

builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();

builder.Services.AddScoped<INewsApiFactory, NewsApiFactory>();
builder.Services.AddScoped<INewsFetcher, NewsFetcherService>();
builder.Services.AddScoped<IUserNotificationService, UserNotificationService>();

builder.Services.AddScoped<IUserArticleActionService, UserArticleActionService>();
builder.Services.AddScoped<IUserArticleActionRepository, UserArticleActionRepository>();

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<EmailNotificationSender>();
builder.Services.AddSingleton<NotificationSenderFactory>();

builder.Services.AddDataProtection();
builder.Services.AddScoped<EncryptionService>();


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<RequestContext>();
builder.Services.AddHostedService<NewsFetchingService>();

builder.Services.AddHttpClient("NewsAPI")
    .AddTransientHttpErrorPolicy(policyBuilder =>
    policyBuilder.WaitAndRetryAsync(
        3,
        retryAttempt => TimeSpan.FromSeconds(2),
        onRetry: (outcome, timespan, retryAttempt, context) =>
        {
            Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds due to {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
        }
    )
);

// Hangfire Config
builder.Services.AddHangfire(x => x.UseInMemoryStorage());
builder.Services.AddHangfireServer();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestContextMiddleware>();

app.MapControllers();

app.Run();
