namespace Temsa.Worker.DynamicAnalysis.Runtime.Frida;

public interface IFridaSession : IAsyncDisposable
{
    IAsyncEnumerable<FridaMessage> ReadMessagesAsync(
        CancellationToken cancellationToken = default);
    
    Task DetachAsync(CancellationToken cancellationToken = default);
}