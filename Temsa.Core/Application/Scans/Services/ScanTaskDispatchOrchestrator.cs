using Temsa.Common.Time;
using Temsa.Core.Application.Scans.Abstractions;
using Temsa.Core.Domain.Entities;
using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Application.Scans.Services;

public class ScanTaskDispatchOrchestrator(
    IScanTaskPublisher scanTaskPublisher,
    ScanTaskDispatchMessageFactory dispatchMessageFactory,
    IDateTimeProvider dateTimeProvider,
    ILogger<ScanTaskDispatchOrchestrator> logger)
{
    private readonly IScanTaskPublisher _scanTaskPublisher = scanTaskPublisher;
    private readonly ScanTaskDispatchMessageFactory _dispatchMessageFactory = dispatchMessageFactory;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<ScanTaskDispatchOrchestrator> _logger = logger;

    public async Task<int> QueueInitialStageAsync(
        Scan scan,
        CancellationToken cancellationToken = default)
    {
        var firstStageOrder = scan.Tasks
            .Where(x => x.Status == ScanTaskStatus.Pending)
            .OrderBy(x => x.StageOrder)
            .Select(x => x.StageOrder)
            .FirstOrDefault();

        if (firstStageOrder == default)
        {
            return 0;
        }

        return await QueueStageAsync(scan, firstStageOrder, cancellationToken);
    }

    public async Task<int> QueueNextAfterTaskCompletedAsync(
        Scan scan,
        ScanTask completedTask,
        CancellationToken cancellationToken = default)
    {
        if (!IsStageComplete(scan, completedTask.StageOrder))
        {
            if (IsSequential(completedTask.StageExecution))
            {
                return await QueueNextPendingTaskInCurrentStageAsync(scan, completedTask, cancellationToken);
            }

            return 0;
        }

        var nextStageOrder = FindNextStageOrder(scan, completedTask.StageOrder, includeAlwaysStages: true);

        if (nextStageOrder is null)
        {
            return 0;
        }

        return await QueueStageAsync(scan, nextStageOrder.Value, cancellationToken);
    }

    public async Task<int> QueueNextAfterTaskFailedAsync(
        Scan scan,
        ScanTask failedTask,
        CancellationToken cancellationToken = default)
    {
        var cleanupStageOrder = scan.Tasks
            .Where(x =>
                x.Status == ScanTaskStatus.Pending &&
                x.StageOrder > failedTask.StageOrder &&
                IsAlwaysRunPolicy(x.RunPolicy))
            .OrderBy(x => x.StageOrder)
            .Select(x => (int?)x.StageOrder)
            .FirstOrDefault();

        if (cleanupStageOrder is null)
        {
            return 0;
        }
        
        return await QueueStageAsync(scan, cleanupStageOrder.Value, cancellationToken);
    }
    
    private async Task<int> QueueStageAsync(
        Scan scan,
        int stageOrder,
        CancellationToken cancellationToken)
    {
        var stageTasks = scan.Tasks
            .Where(x =>
                x.Status == ScanTaskStatus.Pending &&
                x.StageOrder == stageOrder)
            .OrderBy(x => x.Order)
            .ToArray();

        if (stageTasks.Length == 0)
        {
            return 0;
        }

        var execution = stageTasks[0].StageExecution;

        if (IsParallel(execution))
        {
            foreach (var task in stageTasks)
            {
                await QueueTaskAsync(scan, task, cancellationToken);
            }

            return stageTasks.Length;
        }

        var firstTask = stageTasks.First();
        await QueueTaskAsync(scan, firstTask, cancellationToken);
        return 1;
    }

    private async Task<int> QueueNextPendingTaskInCurrentStageAsync(
        Scan scan,
        ScanTask completedTask,
        CancellationToken cancellationToken)
    {
        var nextTask = scan.Tasks
            .Where(x =>
                x.Status == ScanTaskStatus.Pending &&
                x.StageOrder == completedTask.StageOrder)
            .OrderBy(x => x.Order)
            .FirstOrDefault();

        if (nextTask is null)
        {
            return 0;
        }
        
        await QueueTaskAsync(scan, nextTask, cancellationToken);
        return 1;
    }

    private async Task QueueTaskAsync(
        Scan scan,
        ScanTask task,
        CancellationToken cancellationToken)
    {
        var message = _dispatchMessageFactory.Create(scan, task);

        await _scanTaskPublisher.PublishAsync(
            task.WorkerType,
            message,
            cancellationToken);

        task.Status = ScanTaskStatus.Queued;
        task.UpdatedAt = _dateTimeProvider.UtcNow;

        _logger.LogInformation(
            "Queued scan task {ScanTaskId}. " +
            "ScanId = {ScanId}, StageId = {StageId}, StageOrder = {StageOrder}, TaskType = {TaskType}",
            task.Id,
            scan.Id,
            task.StageId,
            task.StageOrder,
            task.TaskType);
    }

    private static bool IsStageComplete(Scan scan, int stageOrder)
    {
        var stageTasks = scan.Tasks
            .Where(x => x.StageOrder == stageOrder)
            .ToArray();

        return stageTasks.Length > 0 &&
               stageTasks.All(x => x.Status == ScanTaskStatus.Completed);
    }

    private static int? FindNextStageOrder(
        Scan scan,
        int currentStageOrder,
        bool includeAlwaysStages)
    {
        return scan.Tasks
            .Where(x =>
                x.Status == ScanTaskStatus.Pending &&
                x.StageOrder > currentStageOrder &&
                (includeAlwaysStages || !IsAlwaysRunPolicy(x.RunPolicy)))
            .OrderBy(x => x.StageOrder)
            .Select(x => (int?)x.StageOrder)
            .FirstOrDefault();
    }

    private static bool IsSequential(string value)
    {
        return string.Equals(value, "sequential", StringComparison.OrdinalIgnoreCase);
    }
    
    private static bool IsParallel(string value)
    {
        return string.Equals(value, "parallel", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAlwaysRunPolicy(string value)
    {
        return string.Equals(value, "always", StringComparison.OrdinalIgnoreCase);
    }
}