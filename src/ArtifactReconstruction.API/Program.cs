using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.RateLimiting;
using ArtifactReconstruction.Application.Interfaces;
using ArtifactReconstruction.Application.Services;
using ArtifactReconstruction.Infrastructure.Meshy;
using ArtifactReconstruction.Infrastructure.OpenAI;

var builder = WebApplication.CreateBuilder(args);


// --- 0. Configuration (Render ENV desteği) ---
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// --- 1. Kontrolcüler ve API Dokümantasyonu ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 2. Uygulama Servisleri (Application Layer) ---
builder.Services.AddScoped<ReconstructionService>();

// --- 3. Altyapı Servisleri (Infrastructure Layer) ---

// Cloudinary Depolama Servisi
builder.Services.AddScoped<IImageStorageService, CloudinaryStorageService>();

// OpenAI Client (Görsel Tamir Servisi)
builder.Services.AddHttpClient<IImageRepairService, OpenAIImageRepairService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
});

// Meshy Client (3D Model Oluşturma Servisi)
builder.Services.AddHttpClient<IMeshy3DService, Meshy3DService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
});

// --- 4. Upload Limit (10MB) ---
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

// --- 5. Rate Limit (API Abuse Protection) ---
builder.Services.AddRateLimiter(opt =>
{
    opt.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
    });
});

// --- 6. CORS Politikası ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// --- 7. Middleware (Ara Yazılım) Yapılandırması ---

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();


app.UseCors("AllowAll");

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
