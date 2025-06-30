using devlife_backend.Models.Games;
using devlife_backend.Models.Request;

namespace devlife_backend.Services
{
    public class CodeAnalyzerService
    {
        public async Task<CodeAnalysisResult> AnalyzeRepositoryAsync(Guid userId, GitHubAnalysisRequest request)
        {
            await Task.Delay(100);

            var personalities = new[]
            {
                "Chaotic Debugger - შენ ხარ კოდის ქაოსური დეტექტივი!",
                "Architecture Wizard - შენ ხარ სისტემური არქიტექტორი!",
                "Speed Demon - შენ ხარ სწრაფი კოდერი!",
                "Perfectionist - შენ ხარ სრულყოფილებისთვის მებრძოლი!"
            };

            return new CodeAnalysisResult
            {
                PersonalityType = personalities[Random.Shared.Next(personalities.Length)],
                Strengths = new[] { "სწრაფი პრობლემის გადაწყვეტა", "კრეატიული მიდგომები" },
                Weaknesses = new[] { "დოკუმენტაცია", "ტესტების წერა" },
                CelebrityMatch = "Linus Torvalds",
                ReposAnalyzed = Random.Shared.Next(1, 10),
                TotalCommits = Random.Shared.Next(50, 500)
            };
        }
    }
}
