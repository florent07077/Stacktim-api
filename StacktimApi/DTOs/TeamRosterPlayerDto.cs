namespace StacktimApi.DTOs;

public class TeamRosterPlayerDto
{
    public int PlayerId { get; set; }
    public string Pseudo { get; set; } = null!;
    public int Role { get; set; } // 0=Captain, 1=Member, 2=Substitute
    public DateTime JoinDate { get; set; }
}