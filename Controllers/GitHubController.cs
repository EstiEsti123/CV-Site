// using Microsoft.AspNetCore.Mvc;
// using CV_SITE.Services;
// using CV_SITE.Models;


// namespace CV_SITE.API
// {
//     [ApiController]
//     [Route("api/[controller]")] // הכתובת תהיה api/github
//     public class GitHubController : ControllerBase
//     {
//         private readonly IGitHubService _gitHubService;

//         // הזרקת השירות (Dependency Injection)
//         public GitHubController(IGitHubService gitHubService)
//         {
//             _gitHubService = gitHubService;
//         }

//         // 1. שליפת הפורטפוליו שלך
//         [HttpGet("portfolio/{username}")]
//         public async Task<ActionResult<List<RepositoryDto>>> GetPortfolio(string username)
//         {
//             var repos = await _gitHubService.GetPortfolioAsync(username);
//             return Ok(repos);
//         }

//         // 2. חיפוש כללי ב-GitHub
//         [HttpGet("search")]
//         public async Task<ActionResult<List<RepositoryDto>>> Search([FromQuery] string? term, [FromQuery] string? language, [FromQuery] string? user)
//         {
//             var results = await _gitHubService.SearchRepositoriesAsync(term, language, user);
//             return Ok(results);
//         }
//     }
// }
using Microsoft.AspNetCore.Mvc;
using CV_SITE.Services;
using CV_SITE.Models;


namespace CV_SITE.API
{
    [ApiController]
  [Route("api/[controller]")]
    public class GitHubController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        public GitHubController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        // --- כאן מתחיל החלק שהיה חסר ---

        [HttpGet("portfolio/{username}")]
        public async Task<ActionResult<List<RepositoryDto>>> GetPortfolio(string username)
        {
            try 
            {
                var repos = await _gitHubService.GetPortfolioAsync(username);
                return Ok(repos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching portfolio: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<RepositoryDto>>> Search([FromQuery] string? term, [FromQuery] string? language, [FromQuery] string? user)
        {
            try
            {
                var results = await _gitHubService.SearchRepositoriesAsync(term, language, user);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error searching repositories: {ex.Message}");
            }
        }
    }
} // ודאי שיש כאן סוגר מסולסל סוגר עבור ה-class ועוד אחד עבור ה-namespace