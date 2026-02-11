using ArtifactReconstruction.Application.Interfaces;

namespace ArtifactReconstruction.Application.Services;

public class ReconstructionService
{
    private readonly IImageRepairService _imageRepair;
    private readonly IMeshy3DService _meshy;

    public ReconstructionService(
        IImageRepairService imageRepair,
        IMeshy3DService meshy)
    {
        _imageRepair = imageRepair;
        _meshy = meshy;
    }

    public async Task<string> ReconstructAsync(byte[] imageBytes, string prompt)
    {
        // Bu artık bize doğrudan Cloudinary linkini (https://res.cloudinary.com/...) dönecek
        var publicUrl = await _imageRepair.RepairAsync(imageBytes);

        // Meshy artık ngrok uyarısına takılmadan bu publicUrl'i indirebilir
        var taskId = await _meshy.Generate3DAsync(publicUrl, prompt);

        var modelUrl = await _meshy.WaitForCompletionAsync(taskId);

        return modelUrl;
    }
}