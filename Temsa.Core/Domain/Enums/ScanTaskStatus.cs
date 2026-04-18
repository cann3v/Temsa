namespace Temsa.Core.Domain.Enums;

public enum ScanTaskStatus
{
    Pending = 1,
    Queued = 2,
    Running = 3,
    Completed = 4,
    Failed = 5,
    Canceled = 6
}