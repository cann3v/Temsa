using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Application.WorkerEvents.Commands.HandleWorkerEvent;

public record HandleWorkerEventResult(
    long ScanId,
    long ScanTaskId,
    ScanStatus ScanStatus,
    ScanTaskStatus ScanTaskStatus,
    DateTimeOffset UpdatedAt);