using ArtifactReconstruction.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ArtifactReconstruction.API.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhooksController : ControllerBase
{
    [HttpPost("meshy")]
    public IActionResult MeshyWebhook([FromBody] MeshyWebhookDto dto)
    {
        if (dto.status != "success")
            return Ok();

        var modelUrl = dto.result.glb_url;

        Console.WriteLine($"🎯 3D MODEL READY: {modelUrl}");

        // TODO:
        // - DB'ye kaydet
        // - Unity client'a bildir (SignalR / WebSocket / Polling)

        return Ok();
    }
}