namespace Temsa.Worker.DynamicAnalysis.Runtime.Scripts;

public record FridaScriptProfileEntry(
    string Id,
    string Path,
    bool Enabled = true);