using System.Text.Json;
using Microsoft.Extensions.Options;
using Temsa.Core.Application.Scans.Abstractions;
using Temsa.Core.Application.Scans.Models;
using Temsa.Core.Configuration;
using Temsa.Core.Domain.Enums;

namespace Temsa.Core.Infrastructure.Pipelines.Json;

public class JsonScanPipelineProvider(
    IOptions<JsonScanPipelineOptions> options,
    IWebHostEnvironment environment,
    ILogger<JsonScanPipelineProvider> logger
    ) : IScanPipelineProvider
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly JsonScanPipelineOptions _options = options.Value;
    private readonly ILogger<JsonScanPipelineProvider> _logger = logger;
    private readonly IWebHostEnvironment _environment = environment;

    public async Task<ScanPipelineDefinition> GetAsync(
        PlatformType platform,
        AnalysisType analysisType,
        CancellationToken cancellationToken = default)
    {
        var filename = $"{platform.ToString().ToLowerInvariant()}.{analysisType.ToString().ToLowerInvariant()}.json)";
        var fullPath = Path.Combine(_environment.ContentRootPath, _options.DirectoryPath, filename);
        
        _logger.LogDebug(
            "Loading scan pipeline definition for {Platform} / {AnalysisType} analysis from {Path}",
            platform,
            analysisType,
            fullPath);

        if (!File.Exists(fullPath))
        {
            throw new InvalidOperationException($"Pipeline definition file was not found: '{fullPath}'");
        }

        await using var stream = File.OpenRead(fullPath);

        var document = await JsonSerializer.DeserializeAsync<RawPipelineDocument>(
            stream,
            JsonSerializerOptions,
            cancellationToken);

        if (document is null)
        {
            throw new InvalidOperationException($"Pipeline definition file '{fullPath}' empty or invalid");
        }

        var resolvedPlatform = ParsePlatform(document.Platform);
        var resolvedAnalysisType = ParseAnalysisType(document.AnalysisType);

        if (resolvedPlatform != platform || resolvedAnalysisType != analysisType)
        {
            throw new InvalidOperationException(
                $"Pipeline definition file '{fullPath}' contains mismatched platform/analysisType");
        }

        var tasks = document.Tasks
            .OrderBy(x => x.Order)
            .Select(x => new ScanPipelineTaskDefinition
            {
                TaskType = RequireValue(x.TaskType, nameof(x.TaskType), fullPath),
                WorkerType = RequireValue(x.WorkerType, nameof(x.WorkerType), fullPath),
                Tool = RequireValue(x.Tool, nameof(x.Tool), fullPath),
                Order = x.Order,
                ParametersJson = x.Parameters.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                    ? null
                    : x.Parameters.GetRawText()
            })
            .ToArray();

        if (tasks.Length == 0)
        {
            throw new InvalidOperationException(
                $"Pipeline definition file '{fullPath}' does not contain any tasks");
        }

        return new ScanPipelineDefinition
        {
            Platform = resolvedPlatform,
            AnalysisType = resolvedAnalysisType,
            Tasks = tasks
        };
    }

    private static PlatformType ParsePlatform(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            "android" => PlatformType.Android,
            "ios" => PlatformType.Ios,
            _ => throw new InvalidOperationException($"Unsupported platform value '{value}'")
        };
    }

    private static AnalysisType ParseAnalysisType(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            "static" => AnalysisType.Static,
            "dynamic" => AnalysisType.Dynamic,
            _ => throw new InvalidOperationException($"Unsupported analysis type '{value}'")
        };
    }

    private static string RequireValue(string? value, string fieldName, string fullPath)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Pipeline definition file '{fullPath}' contains empty field '{fieldName}'");
        }

        return value.Trim();
    }

    private class RawPipelineDocument
    {
        public string? Platform { get; init; }
        public string? AnalysisType { get; init; }
        public List<RawPipelineTaskDocument> Tasks { get; init; } = [];
    }

    private class RawPipelineTaskDocument
    {
        public string? TaskType { get; init; }
        public string? WorkerType { get; init; }
        public string? Tool { get; init; }
        public int Order { get; init; }
        public JsonElement Parameters { get; init; }
    }
}