using Microsoft.EntityFrameworkCore;
using UsermanagementApi.Data;
using UsermanagementApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//  Add Controllers
builder.Services.AddControllers();

//  Register Services
builder.Services.AddScoped<UserService>();

//  Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ================= JWT CONFIG START =================

//  Read Key
var key = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(key))
{
    throw new Exception("JWT Key is missing in appsettings.json");
}

//  Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // for local testing
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key))
    };
});

//  Add Authorization
builder.Services.AddAuthorization();

// ================= JWT CONFIG END =================

var app = builder.Build();

//  Middleware order is IMPORTANT
app.UseHttpsRedirection();

app.UseAuthentication();   //  MUST come before Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();

