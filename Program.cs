using daytask.Data;
using daytask.Dtos;
using daytask.Filters;
using daytask.Middleware;
using daytask.Options;
using daytask.Repositories;
using daytask.Services;
using daytask.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders(); // Optional: Clears default providers
builder.Logging.AddConsole(); // Logs to the console
builder.Logging.AddDebug(); // Logs to the debug output window (useful in Visual Studio)

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Add Rate Limit Based on Authenticated User ID
builder.Services.AddRateLimiter(options =>
{
    // Global limiter
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        return RateLimitPartition.GetTokenBucketLimiter("global", _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 100, // Max 100 requests globally
            TokensPerPeriod = 20, // Refill 20 tokens
            ReplenishmentPeriod = TimeSpan.FromSeconds(60),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 50
        });
    });

    // Per-user limiter
    options.AddPolicy("per-user", context =>
    {
        var userId = context.User?.Identity?.IsAuthenticated == true
            ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous"
            : "anonymous";

        return RateLimitPartition.GetTokenBucketLimiter(userId, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 20,                   // max 10 requests
            TokensPerPeriod = 5,               // refill 1 token
            ReplenishmentPeriod = TimeSpan.FromSeconds(60), // every 6 seconds
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 5
        });
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Register FluentValidation validators
builder.Services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestDtoValidator>();
builder.Services.AddScoped<IValidator<UserDto>, UserDtoValidator>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = string.Empty;
if (builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
} else
{
    connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
     .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information)
     );

// Quartz configuration
builder.Services.AddQuartz(q =>
{
    q.UsePersistentStore(options =>
    {
        options.UseProperties = true;
        options.UseSqlServer(sql =>
        {
            sql.ConnectionString = connectionString;
            sql.TablePrefix = "QRTZ_";
        });
        options.UseNewtonsoftJsonSerializer();
    });
});

builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddHttpClient();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<ITelegramService, TelegramService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();

// Register config binding
builder.Services.Configure<TelegramBotOptions>(builder.Configuration.GetSection("TelegramBot"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => "DayTask APIs v3.22");

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

// Add global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization()
    .RequireRateLimiting("per-user");

app.Run();
