namespace Temsa.Worker.DynamicAnalysis.Runtime.Frida;

public interface IFridaClient
{
    Task<IFridaSession> SpawnAsync(
        FridaSpawnOptions options,
        CancellationToken cancellationToken = default);
}