namespace Temsa.Worker.Runtime.Abstractions;

public interface IWorkerIdentityProvider
{
    string WorkerId { get; }
}