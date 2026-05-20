namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidPrivateDataDump;

public record AndroidPrivateDataDumpPipelineParameters(
    string PackageName,
    bool IncludeCache = true);