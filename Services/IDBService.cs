using apbd_kolos1_poprawka.Models.DTOs;
using apbd_kolos1_poprawka.Models.RequestDTOs;

namespace apbd_kolos1_poprawka.Services;

public interface IDBService
{
    public Task<PreservationProjectDTO> GetPreservationProjectByIdAsync(int id);
    public Task AddNewArtifactAsync(ArtifactPostRequestDTO artifact);
}
