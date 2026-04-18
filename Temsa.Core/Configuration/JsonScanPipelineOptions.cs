namespace Temsa.Core.Configuration;

public class JsonScanPipelineOptions
{
    public const string SectionName = "ScanPipelines";

    public string DirectoryPath { get; init; } = "Pipelines";
}