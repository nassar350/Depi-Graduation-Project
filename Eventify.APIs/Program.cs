using DotNetEnv;
using Eventify.API.Services.Auth;
using Eventify.Core.Entities;
using Eventify.Repository.Data.Contexts;
using Eventify.Service.DependencyInjection;
using Eventify.Service.Helpers;
using Eventify.Service.Interfaces;
using Eventify.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OnlineDbConnectionString");

Env.Load();

var stripeKey = builder.Configuration["STRIPE_SECRET_KEY"] ?? Environment.GetEnvironmentVariable("SecretKey");

if (string.IsNullOrEmpty(stripeKey))
{
    throw new Exception("Stripe secret key is not configured in STRIPE_SECRET_KEY");
}

StripeConfiguration.ApiKey = stripeKey;

// Register ticket services
builder.Services.AddScoped<ITicketEncryptionService>(sp =>
    new TicketEncryptionService(builder.Configuration["TicketEncryption:Key"]));

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
                // Static HTML/CSS/JS development servers
                "http://localhost:3000", // React/Live Server common port
                "http://localhost:3001", 
                "http://localhost:4200", // Angular default port
                "http://localhost:5000", // .NET/Python simple servers
                "http://localhost:5173", // Vite default port
                "http://localhost:5500", // Live Server VS Code extension
                "http://localhost:8000", // Python http.server
                "http://localhost:8080", // Vue/Webpack dev server
                "http://localhost:8090", // Common alternative port
                "http://127.0.0.1:3000",
                "http://127.0.0.1:5500", // Live Server alternative
                "http://127.0.0.1:8000",
                "http://127.0.0.1:8080",
                "https://localhost:3000",
                "https://localhost:3001",
                "https://localhost:4200",
                "https://localhost:5000",
                "https://localhost:5173",
                "https://localhost:5500",
                "https://localhost:8000",
                "https://localhost:8080",
                "https://localhost:8090",
                "https://eventiifyy.netlify.app"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });

    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
    
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<EventifyContext>()
.AddSignInManager<SignInManager<User>>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<JwtTokenGenerator>();

builder.Services.AddDbContext<EventifyContext>(options =>
{
    options.UseSqlServer(connectionString);
});


builder.Services.AddAutoMapperDependency();
builder.Services.AddServiceLayer();

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var settings = config.GetSection("Cloudinary").Get<CloudinarySettings>();

    var account = new CloudinaryDotNet.Account(
        settings.CloudName,
        settings.ApiKey,
        settings.ApiSecret
    );

    return new CloudinaryDotNet.Cloudinary(account);
});

builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token here}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevelopmentPolicy");
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("FrontendPolicy");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
