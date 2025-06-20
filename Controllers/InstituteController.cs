using apbd_kolos1_poprawka.Models.RequestDTOs;
using apbd_kolos1_poprawka.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd_kolos1_poprawka.Controllers;

[ApiController]
[Route("/api")]
public class InstituteController : ControllerBase
{
    protected IDBService service;

    public InstituteController(IDBService service) { this.service = service; }

    [HttpGet("/api/projects/{id}")]
    public async Task<IActionResult> GetPreservationProjectByIdAsync(int id)
    {
        try
        {
            return this.Ok(await this.service.GetPreservationProjectByIdAsync(id));
        }
        catch (ArgumentNullException e) { return this.NotFound(e.Message); }
        catch (Exception e) { return this.StatusCode(500, e.Message); }
    }

    [HttpPost("/api/artifacts")]
    public async Task<IActionResult> PutArtifactAsync(ArtifactPostRequestDTO artifact)
    {
        try
        {
            await this.service.AddNewArtifactAsync(artifact);

            return this.NoContent();
        }
        catch (ArgumentException e) { return this.BadRequest(e.Message); }
        catch (Exception e) { return this.StatusCode(500, e.Message); }
    }
}
