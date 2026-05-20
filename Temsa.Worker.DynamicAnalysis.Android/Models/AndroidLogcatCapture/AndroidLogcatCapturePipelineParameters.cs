namespace Temsa.Worker.DynamicAnalysis.Android.Models.AndroidLogcatCapture;

public record AndroidLogcatCapturePipelineParameters(
    string PackageName,
    int SessionTimeoutSeconds = 600,
    int ProcessWaitTimeoutSeconds = 30,
    int ProcessPollIntervalMilliseconds = 500);