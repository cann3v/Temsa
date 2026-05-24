using Temsa.Worker.StaticAnalysis.Radare2.Abstractions;
using Temsa.Worker.StaticAnalysis.Radare2.Models;

namespace Temsa.Worker.StaticAnalysis.Radare2.Context;

public class IosBinaryContext(IRadare2Session session)
{
    private IReadOnlyCollection<Radare2Import>? _imports;
    private IReadOnlyCollection<Radare2String>? _strings;

    public IRadare2Session Session { get; } = session;
    
    public async Task<IReadOnlyCollection<Radare2Import>> GetImportsAsync(
        CancellationToken cancellationToken = default)
    {
        return _imports ??= await Session.CmdJsonAsync<IReadOnlyCollection<Radare2Import>>(
            "iij",
            cancellationToken) ?? [];
    }
    
    public async Task<IReadOnlyCollection<Radare2String>> GetStringsAsync(
        CancellationToken cancellationToken = default)
    {
        return _strings ??= await Session.CmdJsonAsync<IReadOnlyCollection<Radare2String>>(
            "izj",
            cancellationToken) ?? [];
    }
}