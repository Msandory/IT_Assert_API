using Inventory_System_API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Inventory_System_API.Services_Interfaces;
using Serilog;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services/repository to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<ComputerCountService>();
builder.Services.AddScoped<LaptopService>();
builder.Services.AddScoped<DesktopService>();
builder.Services.AddScoped<TabletService>();
builder.Services.AddScoped<MobileService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<LicenseService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<ServerSessionStore>();
builder.Services.AddScoped<AdditionDeviceService>();
builder.Services.AddScoped<AccessPointService>();
builder.Services.AddScoped<BiometricDeviceService>();
builder.Services.AddScoped<FirewallService>();
builder.Services.AddScoped<PrinterService>();
builder.Services.AddScoped<TurnstileService>();
builder.Services.AddScoped<WifiControllerService>();
builder.Services.AddScoped<StorageDeviceService>();
builder.Services.AddScoped<TonerService>();


// Configure OwnerService with LDAPPath from configuration
builder.Services.AddScoped<OwnerService>(provider =>
{
    var ldapPath = builder.Configuration.GetValue<string>("LdapPath");
    return new OwnerService(provider.GetRequiredService<DataContex>(), ldapPath);
});

// Configure Entity Framework and SQL Server
builder.Services.AddDbContext<DataContex>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IneventoryData"));
});

// Handle reference loops in JSON serialization
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

    // User settings
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<DataContex>()
    .AddDefaultTokenProviders();

// Configure JWT authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
var Configuration = builder.Configuration;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true; // Set this to true in production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidIssuer = Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = ClaimTypes.NameIdentifier
        };

        // Disable the default login redirect on unauthorized access
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                // Prevent the redirect and just return 401
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
        };
    });

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });

    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://lus-assets:8080") // Specific origin for production
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Configure Swagger/OpenAPI
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

app.UseCors("AllowAll"); // Use the defined CORS policy
app.UseCors("AllowFrontend");
app.UseAuthentication(); // Add this line to enable authentication
app.UseAuthorization();

app.MapControllers();

app.Run();
