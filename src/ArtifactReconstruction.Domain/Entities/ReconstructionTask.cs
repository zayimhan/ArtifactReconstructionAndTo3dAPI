namespace ArtifactReconstruction.Domain.Entities;

public class ReconstructionTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Status { get; set; } = "Pending";
    public string? ResultModelUrl { get; set; }
}