using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Ios.Models.IosDynamicSession;

public record IosDynamicSessionExecutionResult(
    string Status,
    string BundleId,
    int FridaMessagesCount,
    IReadOnlyCollection<ScanArtifactDescriptor> Artifacts);