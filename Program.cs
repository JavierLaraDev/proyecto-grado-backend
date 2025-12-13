using ApiGrado.Data;
using ApiGrado.Mappers;
using ApiGrado.Modelos;
using ApiGrado.Repositorio;
using ApiGrado.Repositorio.IRepositorio;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// 1️⃣ BASE DE DATOS
// =====================================================================
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
{
    opciones.UseNpgsql(builder.Configuration.GetConnectionString("ConexionSql"));
});


// =====================================================================
// 2️⃣ REPOSITORIOS
// =====================================================================
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IAccesorioRepositorio, AccesorioRepositorio>();
builder.Services.AddScoped<IPedidosRepositorio, PedidoRepositorio>();

// =====================================================================
// 3️⃣ LÍMITES DE SUBIDA
// =====================================================================
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50 MB
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 50 * 1024 * 1024;
});

// =====================================================================
// 4️⃣ JWT
// =====================================================================
var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();

// =====================================================================
// 5️⃣ CORS
// =====================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("PolicyCors", build =>
    {
        build.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// =====================================================================
// 6️⃣ CONTROLLERS + JSON
// =====================================================================
builder.Services.AddControllers().AddNewtonsoftJson();

// =====================================================================
// 7️⃣ SWAGGER
// =====================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================================================================
// 8️⃣ AUTOMAPPER
// =====================================================================
builder.Services.AddAutoMapper(typeof(BlogMapper));

// =====================================================================
// 9️⃣ 🔥 BACKBLAZE B2 (S3 Compatible)
// =====================================================================

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = new AmazonS3Config
    {
        ServiceURL = builder.Configuration["Backblaze:Endpoint"], // ❗ EJ: https://s3.us-west-002.backblazeb2.com
        ForcePathStyle = true // obligatorio para B2
    };

    return new AmazonS3Client(
        builder.Configuration["Backblaze:KeyId"],
        builder.Configuration["Backblaze:ApplicationKey"],
        config
    );
});

// =====================================================================
// 10️⃣ BUILD
// =====================================================================
var app = builder.Build();

// =====================================================================
// 11️⃣ SWAGGER
// =====================================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =====================================================================
// 12️⃣ ARCHIVOS ESTÁTICOS (opcional)
// =====================================================================
var modelosPath = Path.Combine(Directory.GetCurrentDirectory(), "Modelos3D");
if (Directory.Exists(modelosPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(modelosPath),
        RequestPath = "/Modelos3D",
        ContentTypeProvider = new FileExtensionContentTypeProvider(
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".glb", "model/gltf-binary" }
            })
    });
}

// =====================================================================
// 13️⃣ PIPELINE
// =====================================================================
app.UseCors("PolicyCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
