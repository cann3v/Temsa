using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Temsa.Core.Api.Contracts.Scans;
using Temsa.Core.Api.Contracts.Scans.Artifacts;
using Temsa.Core.Api.Contracts.Scans.ScanTasks;
using Temsa.Core.Application.Scans.Commands.CreateScan;
using Temsa.Core.Application.Scans.Commands.StartScan;
using Temsa.Core.Application.Scans.Queries.GetScan;
using Temsa.Core.Application.Scans.Queries.ListScanArtifacts;
using Temsa.Core.Application.Scans.Queries.ListScanTaskEvents;

namespace Temsa.Core.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ScansController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateScanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateScanRequest request,
        [FromServices] CreateScanHandler handler,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest();
        }

        try
        {
            var result = await handler.HandleAsync(
                new CreateScanCommand(
                    request.ProjectId,
                    request.InputArtifactId,
                    request.Platform,
                    request.AnalysisType),
                cancellationToken);

            var response = new CreateScanResponse(
                result.ScanId,
                result.Status,
                result.TaskCount,
                result.CreatedAt);

            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { id = response.ScanId },
                value: response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new
            {
                error = ex.Message
            });
        }
    }
    
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GetScanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] long id,
        [FromServices] GetScanHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetScanQuery(id),
            cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        var response = new GetScanResponse(
            ScanId: result.ScanId,
            ProjectId: result.ProjectId,
            InputArtifactId: result.InputArtifactId,
            Platform: result.Platform,
            AnalysisType: result.AnalysisType,
            Status: result.Status,
            CurrentStage: result.CurrentStage,
            ErrorMessage: result.ErrorMessage,
            CreatedAt: result.CreatedAt,
            UpdatedAt: result.UpdatedAt,
            StartedAt: result.StartedAt,
            FinishedAt: result.FinishedAt,
            Tasks: result.Tasks
                .Select(x => new GetScanTaskResponse(
                    Id: x.Id,
                    TaskType: x.TaskType,
                    WorkerType: x.WorkerType,
                    Tool: x.Tool,
                    Order: x.Order,
                    Status: x.Status,
                    Attempt: x.Attempt,
                    ErrorMessage: x.ErrorMessage,
                    CreatedAt: x.CreatedAt,
                    UpdatedAt: x.UpdatedAt,
                    StartedAt: x.StartedAt,
                    FinishedAt: x.FinishedAt))
                .ToArray());

        return Ok(response);
    }

    [HttpPost("{id:long}/start")]
    [ProducesResponseType(typeof(StartScanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Start(
        [FromRoute] long id,
        [FromServices] StartScanHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new StartScanCommand(id),
                cancellationToken);

            var response = new StartScanResponse(
                result.ScanId,
                result.Status,
                result.QueuedTaskCount,
                result.UpdatedAt);

            return Ok(response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("was not found"))
        {
            return NotFound(new
            {
                error = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                error = ex.Message
            });
        }
    }

    [HttpGet("{scanId:long}/tasks/{scanTaskId:long}/events")]
    [ProducesResponseType(typeof(IReadOnlyCollection<GetScanTaskEventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskEvents(
        [FromRoute] long scanId,
        [FromRoute] long scanTaskId,
        [FromServices] ListScanTaskEventsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new ListScanTaskEventsQuery(scanId, scanTaskId),
            cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        var response = result
            .Select(x => new GetScanTaskEventResponse(
                x.Id,
                x.ScanId,
                x.ScanTaskId,
                x.EventType,
                ParsePayloadJson(x.PayloadJson),
                x.CreatedAt))
            .ToArray();

        return Ok(response);
    }

    [HttpGet("{scanId:long}/artifacts")]
    [ProducesResponseType(typeof(IReadOnlyCollection<GetScanArtifactResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArtifacts(
        [FromRoute] long scanId,
        [FromServices] ListScanArtifactsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new ListScanArtifactsQuery(scanId),
            cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        var response = result
            .Select(x => new GetScanArtifactResponse(
                x.Id,
                x.ScanId,
                x.Kind,
                x.Bucket,
                x.ObjectKey,
                x.FileName,
                x.ContentType,
                x.SizeBytes,
                x.CreatedAt))
            .ToArray();

        return Ok(response);
    }

    private static JsonElement? ParsePayloadJson(string? payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            return null;
        }

        using var document = JsonDocument.Parse(payloadJson);
        return document.RootElement.Clone();
    }
}