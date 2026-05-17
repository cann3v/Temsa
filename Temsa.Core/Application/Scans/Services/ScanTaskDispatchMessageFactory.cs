using Temsa.Contracts.Artifacts;
using Temsa.Contracts.Messaging.ScanTasks;
using Temsa.Core.Domain.Entities;

namespace Temsa.Core.Application.Scans.Services;

public class ScanTaskDispatchMessageFactory
{
    public ScanTaskDispatchMessage Create(
        Scan scan,
        ScanTask task)
    {
        var inputArtifact = scan.InputArtifact;

        if (inputArtifact is null)
        {
            throw new InvalidOperationException($"Input artifact with id '{scan.InputArtifactId}' was not found");
        }

        var inputArtifactDescriptor = new ProjectArtifactDescriptor(
            Id: inputArtifact.Id,
            Type: inputArtifact.Type,
            Kind: inputArtifact.Kind,
            Bucket: inputArtifact.Bucket,
            ObjectKey: inputArtifact.ObjectKey,
            FileName: inputArtifact.FileName,
            ContentType: inputArtifact.ContentType,
            SizeBytes: inputArtifact.SizeBytes);
        
        return new ScanTaskDispatchMessage(
            ScanTaskId: task.Id,
            ScanId: scan.Id,
            InputArtifact: inputArtifactDescriptor,
            Platform: scan.Platform.ToString().ToLowerInvariant(),
            TaskType: task.TaskType,
            Tool: task.Tool,
            ParametersJson: task.PayloadJson);
    }
}