namespace apbd_kolos1_poprawka.Models.RequestDTOs;

public class ArtifactCreateRequestDTO
{
    public int ArtifactId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime OriginDate { get; set; }
    public int InstitutionId { get; set; }
}
