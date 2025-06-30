namespace devlife_backend.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid? GetUserId(this HttpContext context)
        {
            var userIdString = context.Session.GetString("UserId");
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }

        public static void SetUserSession(this HttpContext context, Guid userId, string sessionToken)
        {
            context.Session.SetString("UserId", userId.ToString());
            context.Session.SetString("SessionToken", sessionToken);
        }

        public static bool IsAuthenticated(this HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Session.GetString("UserId"));
        }
    }
}
