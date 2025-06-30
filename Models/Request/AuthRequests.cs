namespace devlife_backend.Models.Request
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string TechStack { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = "Middle";
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
    }
}
