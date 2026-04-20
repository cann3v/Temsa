using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Application.Scans.Models;

public record ScanTaskDispatchMessage(
    long ScanTaskId,
    long ScanId,
    long InputArtifactId,
    PlatformType Platform,
    string TaskType,
    string Tool,
    string? ParametersJson);