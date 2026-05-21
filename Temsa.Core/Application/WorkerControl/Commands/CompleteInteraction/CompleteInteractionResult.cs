namespace Temsa.Core.Application.WorkerControl.Commands.CompleteInteraction;

public record CompleteInteractionResult(
    long ScanId,
    long? ScanTaskId,
    string CommandType,
    DateTimeOffset OccuredAt);