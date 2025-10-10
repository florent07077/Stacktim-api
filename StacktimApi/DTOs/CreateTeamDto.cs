using System.ComponentModel.DataAnnotations;

namespace StacktimApi.DTOs;

public class CreateTeamDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(3, MinimumLength = 3)]
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Le tag doit contenir exactement 3 lettres majuscules.")]
    public string Tag { get; set; } = null!;

    [Required(ErrorMessage = "CaptainId est requis.")]
    public int CaptainId { get; set; }
}
