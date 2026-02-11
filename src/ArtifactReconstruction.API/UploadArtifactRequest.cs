namespace ArtifactReconstruction.API;


public class UploadArtifactRequest
{
    public IFormFile File { get; set; } = default!;
    public string Prompt { get; set; } = "";
}
