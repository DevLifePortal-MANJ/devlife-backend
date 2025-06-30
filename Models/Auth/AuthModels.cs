namespace devlife_backend.Models.Auth
{
    public class LoginResult
    {
        public User User { get; set; } = null!;
        public string SessionToken { get; set; } = string.Empty;
    }
}
