using System.Text;
using MedisoftAPI.Application.Interfaces;
using MedisoftAPI.Application.UseCases;
using MedisoftAPI.Domain.Interfaces;
using MedisoftAPI.Infrastructure.Data;
using MedisoftAPI.Infrastructure.Repositories;
using MedisoftAPI.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── 0. Crear carpeta Database ANTES de que EF Core intente conectar ─
//       Sin esto, SQLite falla con "unable to open database file"
//       porque la carpeta Database/ no existe en el primer arranque.
var sqliteConn = builder.Configuration.GetConnectionString("SQLite")
    ?? "Data Source=Database/medisoft_users.db";

var dbFile = sqliteConn.Replace("Data Source=", "").Trim();
if (!Path.IsPathRooted(dbFile))
    dbFile = Path.Combine(Directory.GetCurrentDirectory(), dbFile);

var dbFolder = Path.GetDirectoryName(dbFile);
if (!string.IsNullOrWhiteSpace(dbFolder) && !Directory.Exists(dbFolder))
    Directory.CreateDirectory(dbFolder);

// ── 1. SQLite ─────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(sqliteConn));

// ── 2. Repositorios y servicios ──────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFaservicioRepository, FaservicioRepository>();
builder.Services.AddScoped<IGemedicosRepository, GemedicosRepository>();
builder.Services.AddScoped<IAdpacienteRepository, AdpacienteRepository>();
builder.Services.AddScoped<IAddispmedRepository, AddispmedRepository>();
builder.Services.AddScoped<IGeespecialRepository, GeespecialRepository>();
builder.Services.AddScoped<ICtadministRepository, CtadministRepository>();
builder.Services.AddScoped<ICtcontratoRepository, CtcontratoRepository>();
builder.Services.AddScoped<IAdadmipaciRepository, AdadmipaciRepository>();
builder.Services.AddScoped<IAdcitasRepository, AdcitasRepository>();
builder.Services.AddScoped<IConsecutivosRepository, ConsecutivosRepository>();
builder.Services.AddScoped<IFaprogpypRepository, FaprogpypRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFaservicioService, FaservicioService>();
builder.Services.AddScoped<IGemedicosService, GemedicosService>();
builder.Services.AddScoped<IAdpacienteService, AdpacienteService>();
builder.Services.AddScoped<IAddispmedService, AddispmedService>();
builder.Services.AddScoped<IGeespecialService, GeespecialService>();
builder.Services.AddScoped<ICtadministService, CtadministService>();
builder.Services.AddScoped<ICtcontratoService, CtcontratoService>();
builder.Services.AddScoped<IAdadmipaciService, AdadmipaciService>();
builder.Services.AddScoped<IAdcitasService, AdcitasService>();
builder.Services.AddScoped<IFaprogpypService, FaprogpypService>();

// ── 3. JWT ────────────────────────────────────────────────────────
var jwt = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwt["SecretKey"]!);

builder.Services
    .AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ── 4. Swagger ────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Medisoft API",
        Version = "v1",
        Description = "API REST .NET 8 — Integración con Visual FoxPro (MedisoftCore)\n\n" +
                      "⚠️ Para autenticarse: haga login, copie el token y péguelo " +
                      "en el botón Authorize (solo el token, SIN escribir 'Bearer')."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Pegue aquí SOLO el token JWT (sin escribir 'Bearer'). " +
                       "Ejemplo: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xml = Path.Combine(AppContext.BaseDirectory,
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xml)) c.IncludeXmlComments(xml);
});

// ── 5. Controllers ────────────────────────────────────────────────
builder.Services.AddControllers();

var app = builder.Build();

// ── 6. Seed automático ───────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DatabaseSeeder.SeedAsync(db);
}

// ── 7. Pipeline ───────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medisoft API v1");
    c.RoutePrefix = string.Empty;
    c.DocumentTitle = "Medisoft API";
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();