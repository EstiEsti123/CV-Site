using CV_SITE.Models;

namespace CV_SITE.Services
{
    public interface IGitHubService
    {
        // שליפת פורטפוליו למשתמש
        Task<List<RepositoryDto>> GetPortfolioAsync(string userName);
        
        // חיפוש חופשי ב-GitHub
        Task<List<RepositoryDto>> SearchRepositoriesAsync(string? term, string? language, string? user);
    }
}