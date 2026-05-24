using Temsa.Worker.StaticAnalysis.Radare2.Context;
using Temsa.Worker.StaticAnalysis.Radare2.Models;

namespace Temsa.Worker.StaticAnalysis.Radare2.Abstractions;

public interface IRadare2Analyzer
{
    string Id { get; }

    Task<IReadOnlyCollection<Radare2Finding>> AnalyzeAsync(
        IosBinaryContext context,
        CancellationToken cancellationToken = default);
}