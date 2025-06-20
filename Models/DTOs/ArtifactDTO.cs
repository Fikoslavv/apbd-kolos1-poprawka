namespace apbd_kolos1_poprawka.Models.DTOs;

public class ArtifactDTO
{
    public int ArtifactId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime OriginDate { get; set; }
    public InstitutionDTO? Institution { get; set; }
}
