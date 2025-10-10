namespace StacktimApi.DTOs;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Tag { get; set; } = null!;
    public int? CaptainId { get; set; }
    public DateTime CreationDate { get; set; }
}
