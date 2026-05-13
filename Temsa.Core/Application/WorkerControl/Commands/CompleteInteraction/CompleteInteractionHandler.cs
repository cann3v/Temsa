using Microsoft.EntityFrameworkCore;
using Temsa.Common.Time;
using Temsa.Contracts.Messaging.WorkerControl;
using Temsa.Core.Application.WorkerControl.Abstractions;
using Temsa.Core.Domain.Enums;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.WorkerControl.Commands.CompleteInteraction;

public class CompleteInteractionHandler(
    TemsaDbContext dbContext,
    IWorkerControlPublisher publisher,
    IDateTimeProvider dateTimeProvider,
    ILogger<CompleteInteractionHandler> logger)
{
    private readonly TemsaDbContext _dbContext = dbContext;
    private readonly IWorkerControlPublisher _publisher = publisher;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<CompleteInteractionHandler> _logger = logger;

    public async Task<CompleteInteractionResult> HandleAsync(
        CompleteInteractionCommand command,
        CancellationToken cancellationToken = default)
    {
        var scanTask = await _dbContext.ScanTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == command.ScanTaskId &&
                     x.ScanId == command.ScanId,
                cancellationToken);

        if (scanTask is null)
        {
            throw new InvalidOperationException(
                $"Scan task with id '{command.ScanTaskId}' was not found in scan '{command.ScanId}'");
        }
        
        if (scanTask.Status != ScanTaskStatus.Running)
        {
            throw new InvalidOperationException(
                $"Scan task '{command.ScanTaskId}' cannot complete interaction from status '{scanTask.Status}'");
        }
        
        if (!IsDynamicWorker(scanTask.WorkerType))
        {
            throw new InvalidOperationException(
                $"Scan task '{command.ScanTaskId}' is not an interactive dynamic analysis task");
        }
        
        var message = new WorkerControlMessage(
            ScanId: command.ScanId,
            ScanTaskId: command.ScanTaskId,
            CommandType: WorkerControlCommandTypes.InteractionCompleted,
            OccuredAt: _dateTimeProvider.UtcNow);
        
        await _publisher.PublishAsync(
            routingKey: scanTask.WorkerType,
            message,
            cancellationToken);
        
        _logger.LogInformation(
            "Interaction completion command published for scan {ScanId}, task {ScanTaskId}",
            command.ScanId,
            command.ScanTaskId);

        return new CompleteInteractionResult(
            command.ScanId,
            command.ScanTaskId,
            message.CommandType,
            _dateTimeProvider.UtcNow);
    }
    
    private static bool IsDynamicWorker(string workerType)
    {
        return string.Equals(workerType, "android-dynamic-analysis", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(workerType, "ios-dynamic-analysis", StringComparison.OrdinalIgnoreCase);
    }
}