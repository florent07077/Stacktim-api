using System.ComponentModel.DataAnnotations;

namespace StacktimApi.DTOs
{
    public class UpdatePlayerDto
    {
        [StringLength(50, MinimumLength = 3)]
        public string? Pseudo { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [RegularExpression("^(Bronze|Silver|Gold|Platinum|Diamond|Master)$",
            ErrorMessage = "Rank must be one of: Bronze, Silver, Gold, Platinum, Diamond, Master")]
        public string? Rank { get; set; }

        public int? TotalScore { get; set; } // Optionnel : permet de modifier le score
    }
}
