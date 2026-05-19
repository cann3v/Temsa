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
        var filename = $"{platform.ToString().ToLowerInvariant()}.{analysisType.ToString().ToLowerInvariant()}.json";
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

        var tasks = BuildTaskDefinitions(document, fullPath);

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

    private static string? MergeParameters(
        JsonElement pipelineParameters,
        JsonElement stageParameters,
        JsonElement taskParameters,
        string fullPath)
    {
        var merged = new Dictionary<string, JsonElement>(
            StringComparer.OrdinalIgnoreCase);

        MergeInto(merged, pipelineParameters, fullPath, "pipeline parameters");
        MergeInto(merged, stageParameters, fullPath, "stage parameters");
        MergeInto(merged, taskParameters, fullPath, "task parameters");

        return merged.Count == 0
            ? null
            : JsonSerializer.Serialize(merged, JsonSerializerOptions);
    }

    private static void MergeInto(
        Dictionary<string, JsonElement> target,
        JsonElement source,
        string fullPath,
        string sourceName)
    {
        if (source.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            return;
        }

        if (source.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException(
                $"Pipeline definition file '{fullPath}' contains non-object {sourceName}");
        }

        foreach (var property in source.EnumerateObject())
        {
            target[property.Name] = property.Value.Clone();
        }
    }

    private static ScanPipelineTaskDefinition[] BuildTaskDefinitions(
        RawPipelineDocument document,
        string fullPath)
    {
        if (document.Stages.Count == 0)
        {
            throw new InvalidOperationException(
                $"Pipeline definition file '{fullPath}' does not contain any stages");
        }

        return BuildStageTaskDefinitions(document, fullPath);
    }

    private static ScanPipelineTaskDefinition[] BuildStageTaskDefinitions(
        RawPipelineDocument document,
        string fullPath)
    {
        return document.Stages
            .OrderBy(x => x.Order)
            .SelectMany(stage =>
            {
                var stageId = RequireValue(stage.Id, nameof(stage.Id), fullPath);
                var stageExecution = string.IsNullOrWhiteSpace(stage.Execution)
                    ? "sequential"
                    : stage.Execution.Trim();

                var runPolicy = string.IsNullOrWhiteSpace(stage.RunPolicy)
                    ? "on-success"
                    : stage.RunPolicy.Trim();
                
                if (stage.Order <= 0)
                {
                    throw new InvalidOperationException(
                        $"Pipeline definition file '{fullPath}' contains stage '{stageId}' with invalid order");
                }

                if (stage.Tasks.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"Pipeline definition file '{fullPath}' contains stage '{stageId}' without tasks");
                }
                
                if (!string.Equals(stageExecution, "sequential", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(stageExecution, "parallel", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"Pipeline definition file '{fullPath}' contains stage '{stageId}' " +
                        $"with invalid execution '{stageExecution}'");
                }

                return stage.Tasks
                    .OrderBy(task => task.Order)
                    .Select(task => new ScanPipelineTaskDefinition
                    {
                        StageId = stageId,
                        StageOrder = stage.Order,
                        StageExecution = stageExecution,
                        RunPolicy = runPolicy,
                        TaskType = RequireValue(task.TaskType, nameof(task.TaskType), fullPath),
                        WorkerType = RequireValue(task.WorkerType, nameof(task.WorkerType), fullPath),
                        Tool = RequireValue(task.Tool, nameof(task.Tool), fullPath),
                        Order = task.Order,
                        ParametersJson = MergeParameters(
                            document.Parameters,
                            stage.Parameters,
                            task.Parameters,
                            fullPath)
                    });
            })
            .ToArray();
    }

    private class RawPipelineDocument
    {
        public string? Platform { get; init; }
        public string? AnalysisType { get; init; }
        public List<RawPipelineStageDocument> Stages { get; init; } = [];
        public JsonElement Parameters { get; init; }
    }

    private class RawPipelineTaskDocument
    {
        public string? TaskType { get; init; }
        public string? WorkerType { get; init; }
        public string? Tool { get; init; }
        public int Order { get; init; }
        public JsonElement Parameters { get; init; }
    }
    
    private class RawPipelineStageDocument
    {
        public string? Id { get; init; }
        public int Order { get; init; }
        public string? Execution { get; init; }
        public string? RunPolicy { get; init; }
        public JsonElement Parameters { get; init; }
        public List<RawPipelineTaskDocument> Tasks { get; init; } = [];
    }
}