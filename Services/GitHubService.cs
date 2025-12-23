

using CV_SITE.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options; // חובה להוסיף עבור IOptions

namespace CV_SITE.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpClient;
        private readonly GitHubOptions _options;

        // הזרקת ה-Options בבנאי
        public GitHubService(HttpClient httpClient, IOptions<GitHubOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            _httpClient.BaseAddress = new Uri("https://api.github.com/");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "CV-Site-App");
            
            // שימוש בטוקן מתוך ה-Options
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _options.Token);
        }

        public async Task<List<RepositoryDto>> GetPortfolioAsync(string? username = null)
        {
            // אם לא הועבר שם משתמש בפרמטר, נשתמש בשם מה-Options
            var targetUser = username ?? _options.UserName;

            var response = await _httpClient.GetAsync($"users/{targetUser}/repos");
            response.EnsureSuccessStatusCode();
            var reposJson = await response.Content.ReadAsStringAsync();
            var repos = JsonSerializer.Deserialize<List<JsonElement>>(reposJson);

            var portfolio = new List<RepositoryDto>();
            if (repos == null) return portfolio;

            foreach (var repo in repos)
            {
                var repoName = repo.GetProperty("name").GetString();
                // שימוש ב-targetUser כדי לשמור על עקביות
                var prCount = await GetPullRequestsCount(targetUser, repoName);

                portfolio.Add(MapToDto(repo, prCount));
            }
            return portfolio;
        }

        public async Task<List<RepositoryDto>> SearchRepositoriesAsync(string? term, string? language, string? user)
        {
            var query = "search/repositories?q=";
            if (!string.IsNullOrEmpty(term)) query += term;
            if (!string.IsNullOrEmpty(language)) query += $"+language:{language}";
            if (!string.IsNullOrEmpty(user)) query += $"+user:{user}";

            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();
            var searchResult = await response.Content.ReadFromJsonAsync<JsonElement>();
            var items = searchResult.GetProperty("items").EnumerateArray();

            var results = new List<RepositoryDto>();
            foreach (var item in items)
            {
                results.Add(MapToDto(item, 0)); 
            }
            return results;
        }

        private async Task<int> GetPullRequestsCount(string owner, string repoName)
        {
            var prResponse = await _httpClient.GetAsync($"repos/{owner}/{repoName}/pulls?state=all");
            if (prResponse.IsSuccessStatusCode)
            {
                var prsJson = await prResponse.Content.ReadAsStringAsync();
                var prs = JsonSerializer.Deserialize<List<JsonElement>>(prsJson);
                return prs?.Count ?? 0;
            }
            return 0;
        }

        private RepositoryDto MapToDto(JsonElement repo, int prCount)
        {
            return new RepositoryDto
            {
                Name = repo.GetProperty("name").GetString(),
                Language = repo.TryGetProperty("language", out var lang) ? lang.GetString() : "N/A",
                StarsCount = repo.GetProperty("stargazers_count").GetInt32(),
                PullRequestsCount = prCount,
                LastCommit = repo.GetProperty("updated_at").GetDateTimeOffset(),
                RepoUrl = repo.GetProperty("html_url").GetString(),
                Homepage = repo.TryGetProperty("homepage", out var hp) && hp.ValueKind != JsonValueKind.Null 
                           ? hp.GetString() : null
            };
        }
    }
}