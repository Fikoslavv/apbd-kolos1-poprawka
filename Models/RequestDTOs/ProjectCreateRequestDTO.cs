namespace apbd_kolos1_poprawka.Models.RequestDTOs;

public class ProjectCreateRequestDTO
{
    public int ProjectId { get; set; }
    public string Objective { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
