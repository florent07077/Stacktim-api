namespace StacktimApi.DTOs;

public class TeamRosterDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public string Tag { get; set; } = null!;
    public IEnumerable<TeamRosterPlayerDto> Players { get; set; } = new List<TeamRosterPlayerDto>();
}