using Temsa.Worker.Runtime.Abstractions;

namespace Temsa.Worker.Runtime.Identity;

public class DefaultWorkerIdentityProvider : IWorkerIdentityProvider
{
    public string WorkerId { get; } = $"{Environment.MachineName}-{Guid.NewGuid():N}";
}