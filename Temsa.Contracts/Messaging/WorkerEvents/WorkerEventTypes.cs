namespace Temsa.Contracts.Messaging.WorkerEvents;

public class WorkerEventTypes
{
    public const string TaskStarted = "scan-task.started";
    public const string TaskCompleted = "scan-task.completed";
    public const string TaskFailed = "scan-task.failed";
    public const string TaskProgress = "scan-task.progress";
    public const string TaskLog = "scan-task.log";
}