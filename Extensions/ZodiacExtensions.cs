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
                (3, >= 21) or (4, <= 19) => "Aries",        // Aries
                (4, >= 20) or (5, <= 20) => "Taurus",       // Taurus  
                (5, >= 21) or (6, <= 20) => "Gemini",       // Gemini
                (6, >= 21) or (7, <= 22) => "Cancer",       // Cancer
                (7, >= 23) or (8, <= 22) => "Leo",          // Leo
                (8, >= 23) or (9, <= 22) => "Virgo",        // Virgo
                (9, >= 23) or (10, <= 22) => "Libra",       // Libra
                (10, >= 23) or (11, <= 21) => "Scorpio",    // Scorpio
                (11, >= 22) or (12, <= 21) => "Sagittarius", // Sagittarius
                (12, >= 22) or (1, <= 19) => "Capricorn",   // Capricorn
                (1, >= 20) or (2, <= 18) => "Aquarius",     // Aquarius
                (2, >= 19) or (3, <= 20) => "Pisces",       // Pisces
                _ => "unknown"
            };
        }

        public static double GetZodiacLuckMultiplier(this string zodiacSign)
        {
            return zodiacSign.ToLower() switch
            {
                "aries" => 1.2,      // Aries - leadership bonus
                "taurus" => 1.0,     // Taurus - steady
                "gemini" => 1.1,     // Gemini - communication bonus
                "cancer" => 0.9,     // Cancer - emotional penalty
                "leo" => 1.3,        // Leo - confidence bonus
                "virgo" => 1.1,      // Virgo - precision bonus
                "libra" => 1.0,      // Libra - balanced
                "scorpio" => 1.4,    // Scorpio - intensity bonus
                "sagittarius" => 1.2, // Sagittarius - adventure bonus
                "capricorn" => 1.1,  // Capricorn - discipline bonus
                "aquarius" => 1.2,   // Aquarius - innovation bonus
                "pisces" => 0.8,     // Pisces - dreamy penalty
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
                "aries" => "♈",        // Aries
                "taurus" => "♉",       // Taurus
                "gemini" => "♊",       // Gemini
                "cancer" => "♋",       // Cancer
                "leo" => "♌",          // Leo
                "virgo" => "♍",        // Virgo
                "libra" => "♎",        // Libra
                "scorpio" => "♏",      // Scorpio
                "sagittarius" => "♐", // Sagittarius
                "capricorn" => "♑",    // Capricorn
                "aquarius" => "♒",     // Aquarius
                "pisces" => "♓",       // Pisces
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