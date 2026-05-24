using System.Text.Json.Serialization;

namespace Temsa.Worker.StaticAnalysis.Radare2.Models;

public record Radare2Import(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("plt")] long? Plt,
    [property: JsonPropertyName("bind")] string? Bind,
    [property: JsonPropertyName("type")] string? Type);