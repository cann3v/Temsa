namespace Temsa.Worker.Runtime.Abstractions;

public interface IWorkerTaskEventSink
{
    Task ReportProgressAsync(
        string phase,
        string? message = null,
        int? percent = null,
        CancellationToken cancellationToken = default);
    
    Task ReportLogAsync(
        string message,
        string? level = null,
        CancellationToken cancellationToken = default);
}