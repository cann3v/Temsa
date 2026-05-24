using System.Text.Json.Serialization;

namespace Temsa.Worker.StaticAnalysis.Radare2.Models;

public record Radare2Instruction(
    [property: JsonPropertyName("offset")] long Offset,
    [property: JsonPropertyName("opcode")] string? Opcode,
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("disasm")] string? Disasm);