namespace Temsa.Worker.DynamicAnalysis.Ios.Models.IosSystemLogCapture;

public record IosSystemLogCapturePipelineParameters(
    string BundleId,
    int SessionTimeoutSeconds = 600);