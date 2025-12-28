
namespace CV_SITE.Models
{
    public class RepositoryDto
    {
        public string? Name { get; set; }
        public string? Language { get; set; }
        public int StarsCount { get; set; }
        public DateTimeOffset? LastCommit { get; set; }
        public string? RepoUrl { get; set; }
        public string? Homepage { get; set; }
        public int PullRequestsCount { get; set; }
    }
}
