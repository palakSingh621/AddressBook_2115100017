using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using RepositoryLayer.Context;
using RepositoryLayer.Helper;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CacheLayer.Service;
using CacheLayer.Interface;
using StackExchange.Redis;
using Middleware.GlobalExceptionHandler;
using Middleware.RabbitMQ.Service;
using Middleware.RabbitMQ.Interface;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("Application Starting...");

// Adding Greeting Messages API database
builder.Services.AddDbContext<AddressBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));
// Add services to the container.
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>(); // Registering the global exception filter
});
builder.Services.AddScoped<IAddressBookService, AddressBookService>();
builder.Services.AddScoped<IAddressBookRepository, AddressBookRepository>();
builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddScoped<IUserRL, UserRL>();
builder.Services.AddScoped<JwtTokenHelper>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();
builder.Services.AddSingleton<RabbitMQConsumer>();


// Add NLog to the service collection
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
builder.Host.UseNLog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Redis configuration
var redisConfig = builder.Configuration.GetSection("Redis");
string redisConnectionString = $"{redisConfig["Host"]}:{redisConfig["Port"]}";

// Register Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
var app = builder.Build();

var rabbitConsumer = app.Services.GetRequiredService<RabbitMQConsumer>();
Task.Run(() => rabbitConsumer.StartListening());

// Configure the HTTP request pipeline.

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();