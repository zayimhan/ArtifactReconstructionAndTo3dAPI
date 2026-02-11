namespace ArtifactReconstruction.Application.Interfaces;

public interface IImageRepairService
{
    Task<string> RepairAsync(byte[] imageBytes);
}
