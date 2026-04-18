using Microsoft.AspNetCore.Mvc;
using Temsa.Core.Api.Contracts.Projects;
using Temsa.Core.Application.Projects.Commands.CreateProject;
using Temsa.Core.Application.Projects.Queries.GetProject;
using Temsa.Core.Application.Projects.Queries.ListProjects;

namespace Temsa.Core.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ListProjectsItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromServices] ListProjectsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new ListProjectsQuery(), cancellationToken);
        
        var response = result
            .Select(x =>  new ListProjectsItemResponse(
                x.Id,
                x.Name,
                x.CreatedAt,
                x.UpdatedAt))
            .ToArray();

        return Ok(response);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(CreateProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectRequest request,
        [FromServices] CreateProjectHandler handler,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest();
        }

        try
        {
            var result = await handler.HandleAsync(
                new CreateProjectCommand(request.Name),
                cancellationToken);

            var response = new CreateProjectResponse(
                result.Id,
                result.Name,
                result.CreatedAt);

            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { id = response.Id },
                value: response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                error = ex.Message
            });
        }
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GetProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] long id,
        [FromServices] GetProjectHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetProjectQuery(id),
            cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        var response = new GetProjectResponse(
            result.Id,
            result.Name,
            result.CreatedAt,
            result.UpdatedAt);

        return Ok(response);
    }
}