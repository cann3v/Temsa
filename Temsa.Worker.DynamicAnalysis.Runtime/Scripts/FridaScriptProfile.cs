namespace Temsa.Worker.DynamicAnalysis.Runtime.Scripts;

public record FridaScriptProfile(
    string Name,
    IReadOnlyCollection<FridaScriptProfileEntry> Scripts);