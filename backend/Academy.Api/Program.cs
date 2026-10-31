using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Academy.Api.Data;
using Academy.Api.Data.Entities;
using Academy.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Enterprise Backend Engineering Academy API", Version = "v1" });
});

// Configure SQLite Database
builder.Services.AddDbContext<AcademyDbContext>(options =>
{
    options.UseSqlite("Data Source=academy.db;Cache=Shared;Mode=ReadWriteCreate;Pooling=True");
});

// Register Core Domain Services
builder.Services.AddSingleton<ICodeExecutionService, CodeExecutionService>();
builder.Services.AddSingleton<ITerminalSandboxService, TerminalSandboxService>();
builder.Services.AddSingleton<ICurriculumService, CurriculumService>();

// CORS configuration for Frontend development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Ensure Database Created, Schema Upgraded & Seeded
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AcademyDbContext>();
    AcademyDbContext.EnsureSchemaAndSeed(db);
}

// HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Enterprise Backend Academy v1");
    });
}

app.UseCors("AllowFrontend");
app.MapControllers();

// Ensure unmatched /api/* routes return 404 JSON instead of SPA fallback
app.Map("/api/{**slug}", () => Microsoft.AspNetCore.Http.Results.NotFound(new { message = "API endpoint tidak ditemukan." }));

// Serve built React Frontend if present
string distPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "..", "frontend", "dist"));
if (Directory.Exists(distPath))
{
    var fileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(distPath);
    app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = fileProvider });
    app.UseStaticFiles(new StaticFileOptions { FileProvider = fileProvider });
    app.MapFallbackToFile("index.html", new StaticFileOptions { FileProvider = fileProvider });
}

app.Run();
