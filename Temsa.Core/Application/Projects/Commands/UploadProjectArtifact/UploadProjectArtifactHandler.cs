using Microsoft.EntityFrameworkCore;
using Temsa.Common.Storage;
using Temsa.Common.Time;
using Temsa.Core.Domain.Entities;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.Projects.Commands.UploadProjectArtifact;

public class UploadProjectArtifactHandler(
    TemsaDbContext dbContext,
    IArtifactStorage artifactStorage,
    IDateTimeProvider dateTimeProvider,
    ILogger<UploadProjectArtifactHandler> logger)
{
    private readonly TemsaDbContext _dbContext = dbContext;
    private readonly IArtifactStorage _artifactStorage = artifactStorage;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<UploadProjectArtifactHandler> _logger = logger;

    public async Task<UploadProjectArtifactResult> HandleAsync(
        UploadProjectArtifactCommand command,
        CancellationToken cancellationToken = default)
    {
        var projectExists = await _dbContext.Projects.AnyAsync(
            x => x.Id == command.ProjectId,
            cancellationToken);

        if (!projectExists)
        {
            throw new InvalidOperationException(
                $"Project with id {command.ProjectId} was not found");
        }

        var objectKey = BuildObjectKey(
            command.ProjectId,
            command.Type.ToString().ToLowerInvariant(),
            command.FileName);

        var storedArtifact = await _artifactStorage.UploadAsync(
            command.Content,
            objectKey,
            command.FileName,
            command.ContentType,
            cancellationToken);

        var artifact = new ProjectArtifact
        {
            ProjectId = command.ProjectId,
            Type = command.Type,
            Kind = command.Kind,
            Bucket = storedArtifact.Bucket,
            ObjectKey = storedArtifact.ObjectKey,
            FileName = storedArtifact.FileName,
            ContentType = storedArtifact.ContentType,
            SizeBytes = storedArtifact.SizeBytes,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        _dbContext.ProjectArtifacts.Add(artifact);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "Uploaded project artifact {ArtifactId} for project {ProjectId}",
            artifact.Id,
            artifact.ProjectId);

        return new UploadProjectArtifactResult(
            artifact.Id,
            artifact.ProjectId,
            artifact.Type,
            artifact.Kind,
            artifact.Bucket,
            artifact.ObjectKey,
            artifact.FileName,
            artifact.ContentType,
            artifact.SizeBytes,
            artifact.CreatedAt);
    }

    private static string BuildObjectKey(
        long projectId,
        string artifactType,
        string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        
        return $"projects/{projectId}/inputs/{artifactType}/{Guid.NewGuid():N}/{safeFileName}";
    }
}