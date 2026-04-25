using Temsa.Contracts.Messaging.WorkerEvents;

namespace Temsa.Core.Application.WorkerEvents.Commands.HandleWorkerEvent;

public record HandleWorkerEventCommand(
    WorkerEventMessage Message);