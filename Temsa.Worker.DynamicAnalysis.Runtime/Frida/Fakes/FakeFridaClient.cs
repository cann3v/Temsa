using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Temsa.Worker.DynamicAnalysis.Runtime.Frida.Fakes;

public class FakeFridaClient(
    ILogger<FakeFridaClient> logger) : IFridaClient
{
    private readonly ILogger<FakeFridaClient> _logger = logger;

    public Task<IFridaSession> SpawnAsync(
        FridaSpawnOptions options,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Fake Frida spawn for target {TargetIdentifier} with {ScriptCount} scripts",
            options.TargetIdentifier,
            options.Scripts.Count);

        var channel = Channel.CreateUnbounded<FridaMessage>();

        _ = Task.Run(async () =>
        {
            foreach (var script in options.Scripts)
            {
                await channel.Writer.WriteAsync(
                    new FridaMessage(
                        ScriptId: script.Id,
                        Message: $"Fake script loaded: {script.Id}",
                        Data: null),
                    cancellationToken);
            }

            var index = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                index++;

                await channel.Writer.WriteAsync(
                    new FridaMessage(
                        ScriptId: options.Scripts.FirstOrDefault()?.Id ?? "fake",
                        Message: $"Fake Frida runtime message #{index}",
                        Data: null),
                    cancellationToken);

                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }, cancellationToken);

        return Task.FromResult<IFridaSession>(
            new FakeFridaSession(channel, logger: _logger));
    }
}