using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Android.Models;

public record AndroidDynamicSessionExecutionResult(
    string Status,
    string PackageName,
    int FridaMessagesCount,
    IReadOnlyCollection<ScanArtifactDescriptor> Artifacts);