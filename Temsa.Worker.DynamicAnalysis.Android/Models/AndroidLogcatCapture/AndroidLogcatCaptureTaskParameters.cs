using Temsa.Contracts.Artifacts;

namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidLogcatCapture;

public record AndroidLogcatCaptureTaskParameters(
    ProjectArtifactDescriptor InputArtifact,
    string PackageName,
    int SessionTimeoutSeconds,
    int ProcessWaitTimeoutSeconds,
    int ProcessPollIntervalMilliseconds);