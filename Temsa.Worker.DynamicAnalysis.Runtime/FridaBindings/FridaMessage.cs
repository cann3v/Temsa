namespace Temsa.Worker.DynamicAnalysis.Runtime.FridaBindings;

public record FridaMessage(
    string ScriptId,
    string Message,
    byte[]? Data);