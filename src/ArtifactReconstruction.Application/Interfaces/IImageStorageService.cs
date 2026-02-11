namespace ArtifactReconstruction.Application.Interfaces;

public interface IImageStorageService
{
    Task<string> UploadImageAsync(byte[] imageBytes, string fileName);
}