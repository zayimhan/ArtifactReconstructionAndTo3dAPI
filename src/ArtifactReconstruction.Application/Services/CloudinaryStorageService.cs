using ArtifactReconstruction.Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

public class CloudinaryStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryStorageService(IConfiguration config)
    {
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(byte[] imageBytes, string fileName)
    {
        using var stream = new MemoryStream(imageBytes);
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(fileName, stream),
            PublicId = $"artifacts/{Guid.NewGuid()}",
            Overwrite = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        
        if (uploadResult.Error != null)
            throw new Exception($"Cloudinary Error: {uploadResult.Error.Message}");

        return uploadResult.SecureUrl.ToString();
    }
}