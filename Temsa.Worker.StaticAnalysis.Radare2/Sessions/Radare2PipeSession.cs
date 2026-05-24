using System.Text.Json.Nodes;
using R2Pipe;
using Temsa.Worker.StaticAnalysis.Radare2.Abstractions;

namespace Temsa.Worker.StaticAnalysis.Radare2.Sessions;

public class Radare2PipeSession(string radare2Executable = "radare2") : IRadare2Session
{
    private readonly string _radare2Executable = radare2Executable;
    private IR2Pipe? _pipe;

    public async Task OpenAsync(string binaryPath, CancellationToken cancellationToken = default)
    {
        if (_pipe is not null)
            throw new InvalidOperationException("radare2 session is already opened.");

        _pipe = await CreatePipeAsync(binaryPath, cancellationToken);

        await CmdAsync("e scr.color=false", cancellationToken);
        await CmdAsync("e scr.utf8=false", cancellationToken);
        await CmdAsync("aaa", cancellationToken);
    }
    
    public Task<string> CmdAsync(string command, CancellationToken cancellationToken = default)
    {
        return Pipe.CmdAsync(command, cancellationToken);
    }
    
    public Task<JsonNode?> CmdJsonAsync(string command, CancellationToken cancellationToken = default)
    {
        return Pipe.CmdJsonAsync(command, cancellationToken);
    }
    
    public Task<T?> CmdJsonAsync<T>(string command, CancellationToken cancellationToken = default)
    {
        return Pipe.CmdJsonAsync<T>(command, cancellationToken);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_pipe is null)
            return;

        try
        {
            await _pipe.QuitAsync();
        }
        finally
        {
            _pipe = null;
        }
    }
    
    private IR2Pipe Pipe =>
        _pipe ?? throw new InvalidOperationException("radare2 session is not opened.");
    
    private Task<IR2Pipe> CreatePipeAsync(string binaryPath, CancellationToken cancellationToken)
    {
        var options = new R2PipeOpenOptions
        {
            Radare2Path = _radare2Executable
        };

        return global::R2Pipe.R2Pipe.OpenAsync(binaryPath, options, cancellationToken);
    }
}