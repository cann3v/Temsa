using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Application.Scans.Services;

public class ScanStatusCalculator
{
    public ScanStatus Calculate(IReadOnlyCollection<ScanTaskStatus> taskStatuses)
    {
        if (taskStatuses.Count == 0)
        {
            return ScanStatus.Pending;
        }

        if (taskStatuses.All(x => x == ScanTaskStatus.Queued))
        {
            return ScanStatus.Queued;
        }

        if (taskStatuses.Any(x => x == ScanTaskStatus.Running))
        {
            return ScanStatus.Running;
        }

        if (taskStatuses.All(x => x == ScanTaskStatus.Completed))
        {
            return ScanStatus.Completed;
        }

        var terminalStatuses = new[]
        {
            ScanTaskStatus.Completed,
            ScanTaskStatus.Failed,
            ScanTaskStatus.Cancelled
        };

        if (taskStatuses.All(x => terminalStatuses.Contains(x)) &&
            taskStatuses.Any(x => x == ScanTaskStatus.Failed))
        {
            return ScanStatus.Failed;
        }

        if (taskStatuses.All(x => x == ScanTaskStatus.Cancelled))
        {
            return ScanStatus.Cancelled;
        }

        return ScanStatus.Pending;
    }
}