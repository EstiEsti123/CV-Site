using Microsoft.Extensions.Caching.Memory;
using CV_SITE.Models;

namespace CV_SITE.Services
{
    public class CachedGitHubService : IGitHubService
    {
        private readonly IGitHubService _innerService;
        private readonly IMemoryCache _memoryCache;
        private const string PortfolioCacheKey = "PortfolioCacheKey";

        public CachedGitHubService(IGitHubService innerService, IMemoryCache memoryCache)
        {
            _innerService = innerService;
            _memoryCache = memoryCache;
        }

        public async Task<List<RepositoryDto>> GetPortfolioAsync(string? username = null)
        {
            // מפתח ה-Cache משתנה לפי שם המשתמש
            string cacheKey = $"{PortfolioCacheKey}_{username ?? "default"}";

            // ניסיון לשלוף מה-Cache
            if (!_memoryCache.TryGetValue(cacheKey, out List<RepositoryDto> portfolio))
            {
                // אם לא קיים, קוראים לשירות האמיתי
                portfolio = await _innerService.GetPortfolioAsync(username);

                // שמירה ב-Cache ל-5 דקות
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _memoryCache.Set(cacheKey, portfolio, cacheOptions);
            }

            return portfolio;
        }

        // בפונקציית החיפוש בדרך כלל לא עושים Cache כי התוצאות משתנות מאוד
        public Task<List<RepositoryDto>> SearchRepositoriesAsync(string? term, string? language, string? user)
        {
            return _innerService.SearchRepositoriesAsync(term, language, user);
        }
    }
}