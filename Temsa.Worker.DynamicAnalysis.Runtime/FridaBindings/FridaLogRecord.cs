using System.Text.Json;

namespace Temsa.Worker.DynamicAnalysis.Runtime.FridaBindings;

public record FridaLogRecord(
    string ScriptId,
    DateTimeOffset OccuredAt,
    string? Type,
    JsonElement Message,
    JsonElement? Payload = null,
    string? DataBase64 = null,
    int? DataLength = null);