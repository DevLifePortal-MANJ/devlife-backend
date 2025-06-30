namespace devlife_backend.Extensions
{
    public static class ZodiacExtensions
    {
        public static string CalculateZodiacSign(this DateTime birthDate)
        {
            var month = birthDate.Month;
            var day = birthDate.Day;

            return (month, day) switch
            {
                (3, >= 21) or (4, <= 19) => "Aries",
                (4, >= 20) or (5, <= 20) => "Taurus",
                (5, >= 21) or (6, <= 20) => "Gemini",
                (6, >= 21) or (7, <= 22) => "Cancer",
                (7, >= 23) or (8, <= 22) => "Leo",
                (8, >= 23) or (9, <= 22) => "Virgo",
                (9, >= 23) or (10, <= 22) => "Libra",
                (10, >= 23) or (11, <= 21) => "Scorpio",
                (11, >= 22) or (12, <= 21) => "Sagittarius",
                (12, >= 22) or (1, <= 19) => "Capricorn",
                (1, >= 20) or (2, <= 18) => "Aquarius",
                (2, >= 19) or (3, <= 20) => "Pisces",
                _ => "unknown"
            };
        }

        public static double GetZodiacLuckMultiplier(this string zodiacSign)
        {
            return zodiacSign.ToLower() switch
            {
                "aries" => 1.2,
                "taurus" => 1.0,
                "gemini" => 1.1,
                "cancer" => 0.9,
                "leo" => 1.3,
                "virgo" => 1.1,
                "libra" => 1.0,
                "scorpio" => 1.4,
                "sagittarius" => 1.2,
                "capricorn" => 1.1,
                "aquarius" => 1.2,
                "pisces" => 0.8,
                _ => 1.0
            };
        }

        public static int CalculatePointsWithZodiac(this string zodiacSign, int basePoints)
        {
            var multiplier = zodiacSign.GetZodiacLuckMultiplier();
            return (int)Math.Round(basePoints * multiplier);
        }

        public static string GetZodiacEmoji(this string zodiacSign)
        {
            return zodiacSign.ToLower() switch
            {
                "aries" => "♈",
                "taurus" => "♉",
                "gemini" => "♊",
                "cancer" => "♋",
                "leo" => "♌",
                "virgo" => "♍",
                "libra" => "♎",
                "scorpio" => "♏",
                "sagittarius" => "♐",
                "capricorn" => "♑",
                "aquarius" => "♒",
                "pisces" => "♓",
                _ => "⭐"
            };
        }

        public static string GetZodiacGreeting(this string zodiacSign)
        {
            return zodiacSign.ToLower() switch
            {
                "aries" => "🔥 ლიდერო",
                "leo" => "👑 მეფევ",
                "scorpio" => "🦂 ძლიერო",
                "sagittarius" => "🏹 მოგზაურო",
                "gemini" => "💫 ჭკვიანო",
                "virgo" => "📚 სრულყოფილო",
                _ => "💻 დეველოპერო"
            };
        }
    }
}