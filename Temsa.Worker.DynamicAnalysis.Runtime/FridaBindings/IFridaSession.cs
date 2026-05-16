namespace Temsa.Worker.DynamicAnalysis.Runtime.FridaBindings;

public interface IFridaSession : IAsyncDisposable
{
    IAsyncEnumerable<FridaMessage> ReadMessagesAsync(
        CancellationToken cancellationToken = default);
    
    Task DetachAsync(CancellationToken cancellationToken = default);
}