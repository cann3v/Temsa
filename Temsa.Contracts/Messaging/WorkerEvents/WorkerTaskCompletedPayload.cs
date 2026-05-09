using System.Text.Json;
using Temsa.Contracts.Artifacts;

namespace Temsa.Contracts.Messaging.WorkerEvents;

public record WorkerTaskCompletedPayload(
    string Status,
    IReadOnlyCollection<ScanArtifactDescriptor> Artifacts,
    JsonElement? Result);