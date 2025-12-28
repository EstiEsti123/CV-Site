using CV_SITE.Services;
using CV_SITE.Models;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<GitHubService>();

builder.Services.AddScoped<IGitHubService>(provider => 
{
    var githubService = provider.GetRequiredService<GitHubService>();
    var memoryCache = provider.GetRequiredService<IMemoryCache>();
    
    return new CachedGitHubService(githubService, memoryCache);
});

builder.Services.AddControllers(); 


builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.Configure<GitHubOptions>(builder.Configuration.GetSection("GitHub"));
var app = builder.Build();
app.UseCors();

// בדיקה אם אנחנו בסביבת פיתוח
if (app.Environment.IsDevelopment())
{
    // השורות הבאות מושבתות כדי שלא יפילו את ההרצה
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); 

app.MapGet("/check", () => "The server is alive and running!");

app.Run();
