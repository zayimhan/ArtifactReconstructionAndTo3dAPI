using System.Net.Http.Headers;
using System.Text.Json;
using ArtifactReconstruction.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ArtifactReconstruction.Infrastructure.OpenAI;

public class OpenAIImageRepairService : IImageRepairService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly IImageStorageService _storageService;

    public OpenAIImageRepairService(HttpClient http, IConfiguration config,IImageStorageService storageService)
    {
        _http = http;
        _config = config;
        _storageService = storageService;

        var apiKey = config["OpenAI:ApiKey"]
                     ?? throw new Exception("OpenAI API Key missing");

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<string> RepairAsync(byte[] imageBytes)
    {
        using var form = new MultipartFormDataContent();

        var imageContent = new ByteArrayContent(imageBytes);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");

        form.Add(imageContent, "image", "artifact.png");

        form.Add(new StringContent(
            "Professional archaeological restoration: Photorealistically reconstruct the missing or broken sections of this specific artifact while strictly preserving the original texture, material grain, and weathered surface details of the existing parts. Ensure historical accuracy in the reconstructed geometry. Use consistent lighting and shadows to match the original fragment. High-detail 8k resolution, museum quality, maintaining the authentic stony/metallic patina."
        ), "prompt");

        form.Add(new StringContent("gpt-image-1"), "model");

        var response = await _http.PostAsync("https://api.openai.com/v1/images/edits", form);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception("OpenAI error → " + json);

        using var doc = JsonDocument.Parse(json);

        var base64 = doc.RootElement.GetProperty("data")[0].GetProperty("b64_json").GetString();
        var bytes = Convert.FromBase64String(base64!);

        
        return await _storageService.UploadImageAsync(bytes, $"{Guid.NewGuid()}.png");
    }
}
