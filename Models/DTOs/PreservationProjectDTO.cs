namespace apbd_kolos1_poprawka.Models.DTOs;

public class PreservationProjectDTO
{
    public int ProjectId { get; set; }
    public ArtifactDTO? Artifact { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Objective { get; set; } = string.Empty;
    public ICollection<StaffAssignmentDTO> StaffAssignments { get; set; } = [];
}
