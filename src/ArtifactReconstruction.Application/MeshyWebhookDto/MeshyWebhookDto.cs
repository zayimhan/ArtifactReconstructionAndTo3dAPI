namespace ArtifactReconstruction.Application.DTOs;

public class MeshyWebhookDto
{
    public string id { get; set; }
    public string status { get; set; }
    public Result result { get; set; }

    public class Result
    {
        public string glb_url { get; set; }
        public string preview_url { get; set; }
    }
}