namespace Temsa.Worker.DynamicAnalysis.Ios.Models.IosAppContainerDump;

public record IosAppContainerDumpPipelineParameters(
    string BundleId,
    bool IncludeTmp = true,
    bool IncludeCaches = true);