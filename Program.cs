using CV_SITE.Services;
using CV_SITE.Models;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
// 1. הוספת ה-Cache למערכת
builder.Services.AddMemoryCache();

// 2. רישום ה-HttpClient עבור GitHubService בצורה הנכונה
// השורה הזו היא הקריטית - היא אומרת לשרת איך להזריק HttpClient לתוך GitHubService
builder.Services.AddHttpClient<GitHubService>();

// 3. רישום הדקורייטור
builder.Services.AddScoped<IGitHubService>(provider => 
{
    // כאן אנחנו מושכים את ה-GitHubService שכבר רשמנו למעלה עם ה-HttpClient שלו
    var githubService = provider.GetRequiredService<GitHubService>();
    var memoryCache = provider.GetRequiredService<IMemoryCache>();
    
    return new CachedGitHubService(githubService, memoryCache);
});
// --- רישום שירותים (DI Container) ---

// הוספת תמיכה בקונטרולרים - קריטי למניעת שגיאת 404
builder.Services.AddControllers(); 

// חיבור ה-Interface למימוש של ה-Service שלנו
// builder.Services.AddHttpClient<IGitHubService, GitHubService>();
// builder.Services.AddScoped<IGitHubService, GitHubService>();

// השורות הבאות מושבתות (במצב הערה) כדי למנוע שגיאות קומפילציה אם לא מותקן Swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.Configure<GitHubOptions>(builder.Configuration.GetSection("GitHub"));
var app = builder.Build();
app.UseCors();
// --- הגדרות צינור בקשות ה-HTTP (Middleware Pipeline) ---

// בדיקה אם אנחנו בסביבת פיתוח
if (app.Environment.IsDevelopment())
{
    // השורות הבאות מושבתות כדי שלא יפילו את ההרצה
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

// אבטחה וניתוב
app.UseHttpsRedirection();
app.UseAuthorization();

// שורת המפתח: מחברת את נתיבי ה-URL לקוד בתיקיית ה-API
app.MapControllers(); 

// נתיב בדיקה מהיר - וודאי שזה עובד בכתובת http://localhost:5104/check
app.MapGet("/check", () => "The server is alive and running!");

app.Run();