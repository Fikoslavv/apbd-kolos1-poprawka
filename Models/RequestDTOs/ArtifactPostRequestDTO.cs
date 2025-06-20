namespace apbd_kolos1_poprawka.Models.RequestDTOs;

public class ArtifactPostRequestDTO
{
    public required ArtifactCreateRequestDTO Artifact { get; set; }
    public required ProjectCreateRequestDTO Project { get; set; }
}
