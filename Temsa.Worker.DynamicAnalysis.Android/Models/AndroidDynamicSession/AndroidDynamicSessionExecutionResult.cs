using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidDynamicSession;

public record AndroidDynamicSessionExecutionResult(
    string Status,
    string PackageName,
    int FridaMessagesCount,
    IReadOnlyCollection<ScanArtifactDescriptor> Artifacts);