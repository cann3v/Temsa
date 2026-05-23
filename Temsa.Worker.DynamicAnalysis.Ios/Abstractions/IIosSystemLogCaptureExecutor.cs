using Temsa.Worker.DynamicAnalysis.Ios.Models.IosSystemLogCapture;
using Temsa.Worker.Runtime.Execution;

namespace Temsa.Worker.DynamicAnalysis.Ios.Abstractions;

public interface IIosSystemLogCaptureExecutor
{
    Task<IosSystemLogCaptureExecutionResult> ExecuteAsync(
        IosSystemLogCaptureTaskParameters parameters,
        WorkerTaskExecutionControl control,
        CancellationToken cancellationToken = default);
}