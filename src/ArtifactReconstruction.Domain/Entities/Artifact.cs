namespace ArtifactReconstruction.Domain.Entities;

public class Artifact
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
}