using System.Text.Json.Serialization;

namespace Temsa.Worker.StaticAnalysis.Radare2.Models;

public record Radare2String(
    [property: JsonPropertyName("vaddr")] long? VirtualAddress,
    [property: JsonPropertyName("paddr")] long? PhysicalAddress,
    [property: JsonPropertyName("string")] string? Value,
    [property: JsonPropertyName("length")] int? Length,
    [property: JsonPropertyName("section")] string? Section);