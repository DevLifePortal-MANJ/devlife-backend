namespace devlife_backend.Models.Request
{
    public class CasinoBetRequest
    {
        public int BetAmount { get; set; }
        public int ChosenOption { get; set; }
        public int CorrectOption { get; set; }
        public string? SnippetId { get; set; }
    }

    public class EndGameRequest
    {
        public Guid SessionId { get; set; }
        public int Score { get; set; }
        public int Distance { get; set; }
        public int BugsCollected { get; set; }
    }

    public class CodeSubmissionRequest
    {
        public string ChallengeId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Language { get; set; } = "javascript";
    }

    public class GitHubAnalysisRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Repository { get; set; } = string.Empty;
    }

    public class SwipeRequest
    {
        public string ProfileId { get; set; } = string.Empty;
        public bool IsLike { get; set; }
    }

    public class ExcuseRequest
    {
        public string Category { get; set; } = string.Empty;
        public int BelieverLevel { get; set; } = 5;
    }
}
