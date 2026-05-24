using System.Text.Json.Serialization;

namespace Temsa.Worker.StaticAnalysis.Radare2.Models;

public record Radare2Xref(
    [property: JsonPropertyName("from")] long From,
    [property: JsonPropertyName("to")] long? To,
    [property: JsonPropertyName("type")] string? Type);