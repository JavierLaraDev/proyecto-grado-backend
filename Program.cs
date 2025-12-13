using Amazon.S3;
using ApiGrado.Data;
using ApiGrado.Mappers;
using ApiGrado.Modelos;
using ApiGrado.Repositorio;
using ApiGrado.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// 1️⃣ BASE DE DATOS (PostgreSQL - Neon)
// =====================================================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConexionSql"));
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
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024;
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 50 * 1024 * 1024;
});

// =====================================================================
// 4️⃣ JWT
// =====================================================================
var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
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
    options.AddPolicy("PolicyCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// =====================================================================
// 6️⃣ CONTROLLERS + JSON
// =====================================================================
builder.Services.AddControllers()
    .AddNewtonsoftJson();

// =====================================================================
// 7️⃣ SWAGGER (HABILITADO EN PRODUCCIÓN)
// =====================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ApiGrado",
        Version = "v1"
    });

    // JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer {token}'"
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
            Array.Empty<string>()
        }
    });
});

// =====================================================================
// 8️⃣ AUTOMAPPER
// =====================================================================
builder.Services.AddAutoMapper(typeof(BlogMapper));

// =====================================================================
// 9️⃣ BACKBLAZE B2
// =====================================================================
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = new AmazonS3Config
    {
        ServiceURL = builder.Configuration["Backblaze:Endpoint"],
        ForcePathStyle = true
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
// 11️⃣ SWAGGER (SIN IF)
// =====================================================================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiGrado v1");
    c.RoutePrefix = "swagger";
});

// =====================================================================
// 12️⃣ ARCHIVOS ESTÁTICOS (Modelos 3D)
// =====================================================================
var modelosPath = Path.Combine(Directory.GetCurrentDirectory(), "Modelos3D");
if (Directory.Exists(modelosPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(modelosPath),
        RequestPath = "/Modelos3D",
        ContentTypeProvider = new FileExtensionContentTypeProvider(
            new Dictionary<string, string>
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
