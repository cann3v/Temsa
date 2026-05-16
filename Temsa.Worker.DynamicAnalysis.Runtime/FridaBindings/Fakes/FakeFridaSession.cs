using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Temsa.Worker.DynamicAnalysis.Runtime.FridaBindings.Fakes;

public class FakeFridaSession(
    Channel<FridaMessage> channel,
    ILogger logger) : IFridaSession
{
    private readonly Channel<FridaMessage> _channel = channel;
    private readonly ILogger _logger = logger;
    
    public async IAsyncEnumerable<FridaMessage> ReadMessagesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return message;
        }
    }

    public Task DetachAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fake Frida session detached");
        _channel.Writer.TryComplete();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _channel.Writer.TryComplete();
        return ValueTask.CompletedTask;
    }
}