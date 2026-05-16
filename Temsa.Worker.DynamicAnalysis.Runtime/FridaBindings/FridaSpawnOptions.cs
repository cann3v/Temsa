namespace Temsa.Worker.DynamicAnalysis.Runtime.FridaBindings;

public record FridaSpawnOptions(
    string TargetIdentifier,
    string? DeviceId,
    IReadOnlyCollection<FridaScriptDefinition> Scripts);