using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Frida;
using Microsoft.Extensions.Logging;

namespace Temsa.Worker.DynamicAnalysis.Runtime.FridaBindings.ClrBindings;

public class FridaClrSession(
    Session session,
    IReadOnlyCollection<Script> scripts,
    BlockingCollection<FridaMessage> messages,
    ILogger logger) : IFridaSession
{
    private readonly Session _session = session;
    private readonly IReadOnlyCollection<Script> _scripts = scripts;
    private readonly BlockingCollection<FridaMessage> _messages = messages;
    private readonly ILogger _logger = logger;
    private bool _disposed;

    public async IAsyncEnumerable<FridaMessage> ReadMessagesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested && !_messages.IsCompleted)
        {
            FridaMessage? message;

            try
            {
                message = _messages.Take(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                yield break;
            }
            catch (InvalidOperationException)
            {
                yield break;
            }

            yield return message;

            await Task.Yield();
        }
    }

    public Task DetachAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            return Task.CompletedTask;
        }

        foreach (var script in _scripts)
        {
            try
            {
                script.Unload();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to unload Frida script");
            }
        }

        try
        {
            _session.Detach();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to detach Frida session");
        }
        
        _messages.CompleteAdding();
        _disposed = true;

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await DetachAsync();
        _messages.Dispose();
    }
}