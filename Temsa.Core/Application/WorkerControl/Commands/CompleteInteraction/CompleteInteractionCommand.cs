namespace Temsa.Core.Application.WorkerControl.Commands.CompleteInteraction;

public record CompleteInteractionCommand(
    long ScanId,
    long? ScanTaskId = null);