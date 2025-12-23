// namespace CV_SITE.Models // החליפי את YourProjectName בשם הפרויקט שלך
// {
//     public class RepositoryDto
//     {
//         public string Name { get; set; }           // שם הפרויקט
//         public string Language { get; set; }       // שפת פיתוח עיקרית
//         public int StarsCount { get; set; }        // כמות כוכבים
//         public DateTimeOffset? LastCommit { get; set; } // תאריך עדכון אחרון
//         public string RepoUrl { get; set; }        // קישור לגיטהאב
//         public string Homepage { get; set; }       // קישור לאתר הפרויקט (אם יש)
//     }
// }
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