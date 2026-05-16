namespace Temsa.Worker.DynamicAnalysis.Runtime.Frida;

public record FridaSpawnOptions(
    string TargetIdentifier,
    string? DeviceId,
    IReadOnlyCollection<FridaScriptDefinition> Scripts);