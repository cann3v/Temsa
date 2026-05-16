namespace Temsa.Worker.DynamicAnalysis.Runtime.Scripts;

public interface IFridaScriptProvider
{
    Task<IReadOnlyCollection<FridaLoadedScript>> LoadScriptAsync(
        string profileName,
        IReadOnlyCollection<string> enabledScripts,
        IReadOnlyCollection<string> disabledScripts,
        CancellationToken cancellationToken = default);
}