using System.Text.Json.Nodes;

namespace Temsa.Worker.StaticAnalysis.Radare2.Abstractions;

public interface IRadare2Session : IAsyncDisposable
{
    Task OpenAsync(string binaryPath, CancellationToken cancellationToken = default);

    Task<string> CmdAsync(string command, CancellationToken cancellationToken = default);

    Task<JsonNode?> CmdJsonAsync(string command, CancellationToken cancellationToken = default);

    Task<T?> CmdJsonAsync<T>(string command, CancellationToken cancellationToken = default);
}