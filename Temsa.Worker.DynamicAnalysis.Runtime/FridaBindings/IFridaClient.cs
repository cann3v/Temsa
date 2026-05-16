namespace Temsa.Worker.DynamicAnalysis.Runtime.FridaBindings;

public interface IFridaClient
{
    Task<IFridaSession> SpawnAsync(
        FridaSpawnOptions options,
        CancellationToken cancellationToken = default);
}