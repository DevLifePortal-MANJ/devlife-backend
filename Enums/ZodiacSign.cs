using System.ComponentModel.DataAnnotations;

namespace devlife_backend.Enums
{
    public enum ZodiacSign
    {
        [Display(Name = "♈")] Aries = 0,
        [Display(Name = "♉")] Taurus,
        [Display(Name = "♊")] Gemini,
        [Display(Name = "♋")] Cancer,
        [Display(Name = "♌")] Leo,
        [Display(Name = "♍")] Virgo,
        [Display(Name = "♎")] Libra,
        [Display(Name = "♏")] Scorpio,
        [Display(Name = "♐")] Sagittarius,
        [Display(Name = "♑")] Capricorn,
        [Display(Name = "♒")] Aquarius,
        [Display(Name = "♓")] Pisces
    }
}
