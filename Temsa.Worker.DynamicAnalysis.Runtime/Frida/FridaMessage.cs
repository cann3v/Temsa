namespace Temsa.Worker.DynamicAnalysis.Runtime.Frida;

public record FridaMessage(
    string ScriptId,
    string Message,
    byte[]? Data);