using FootballPrediction.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Football & Lottery (2D/3D) AI Prediction API", Version = "v1" });
});

// Configure CORS for Blazor WASM client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// HTTP Client for external search and Gemini API calls
builder.Services.AddHttpClient();

// Application Services
builder.Services.AddSingleton<ISettingsService, SettingsService>();
builder.Services.AddScoped<IWebSearchService, WebSearchService>();
builder.Services.AddScoped<IAiPredictionService, AiPredictionService>();
builder.Services.AddScoped<IFootballService, FootballService>();
builder.Services.AddScoped<ILotteryService, LotteryService>();

// Dynamic PORT support for cloud hosts (Render, Koyeb, Railway, etc.)
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Support Blazor WASM framework files, pre-compression (.br, .gz), & MIME types
app.UseBlazorFrameworkFiles();
app.UseDefaultFiles();
var contentTypeProvider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".wasm"] = "application/wasm";
contentTypeProvider.Mappings[".br"] = "application/x-brotli";
contentTypeProvider.Mappings[".gz"] = "application/x-gzip";
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeProvider
});

app.UseAuthorization();
app.MapControllers();

// Health check endpoint (for Cloud deployment liveness probes)
app.MapGet("/health", () => Results.Ok(new { status = "online", service = "FootballPrediction.Api", timestamp = DateTime.UtcNow }));

// Blazor SPA fallback or local dev redirect
var indexFile = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "index.html");
if (File.Exists(indexFile))
{
    app.MapFallbackToFile("index.html");
}
else
{
    app.MapGet("/", () => Results.Redirect("http://localhost:5181/"));
}

app.Run();
