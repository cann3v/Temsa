using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidPrivateDataDump;

public record AndroidPrivateDataDumpTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string PackageName,
    bool IncludeCache = true);