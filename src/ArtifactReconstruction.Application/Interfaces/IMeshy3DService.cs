namespace ArtifactReconstruction.Application.Interfaces;

public interface IMeshy3DService
{
    Task<string> Generate3DAsync(string imageUrl, string prompt);
    Task<string> WaitForCompletionAsync(string taskId);
}
