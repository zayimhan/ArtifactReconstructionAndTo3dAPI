using ArtifactReconstruction.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArtifactReconstruction.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReconstructionController : ControllerBase
{
    private readonly ReconstructionService _service;

    public ReconstructionController(ReconstructionService service)
    {
        _service = service;
    }
    
    [HttpGet("env-test")]
    public IActionResult EnvTest()
    {
        return Ok(new
        {
            meshy = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Meshy__ApiKey")),
            cloud = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Cloudinary__CloudName"))
        });
    }


    
    
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] UploadArtifactRequest request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("File is required");

        using var ms = new MemoryStream();
        await request.File.CopyToAsync(ms);

        var imageBytes = ms.ToArray();

        var modelUrl = await _service.ReconstructAsync(imageBytes, request.Prompt);

        return Ok(new
        {
            success = true,
            modelUrl = modelUrl
        });
    }
}