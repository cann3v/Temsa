using Microsoft.EntityFrameworkCore;
using Temsa.Common.Storage;
using Temsa.Core.Infrastructure.Persistence;

namespace Temsa.Core.Application.Scans.Queries.GetScanArtifactContent;

public class GetScanArtifactContentHandler(
    TemsaDbContext dbContext,
    IArtifactStorage artifactStorage)
{
    private readonly TemsaDbContext _dbContext = dbContext;
    private readonly IArtifactStorage _artifactStorage = artifactStorage;

    public async Task<GetScanArtifactContentResult?> HandleAsync(
        GetScanArtifactContentQuery query,
        CancellationToken cancellationToken = default)
    {
        var artifact = await _dbContext.ScanArtifacts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == query.ArtifactId &&
                     x.ScanId == query.ScanId,
                cancellationToken);

        if (artifact is null)
        {
            return null;
        }

        var stream = new MemoryStream();

        try
        {
            await _artifactStorage.DownloadAsync(
                artifact.Bucket,
                artifact.ObjectKey,
                stream,
                cancellationToken);

            stream.Position = 0;

            return new GetScanArtifactContentResult(
                Content: stream,
                FileName: artifact.FileName ?? $"scan-artirfact-{artifact.Id}",
                ContentType: artifact.ContentType ?? "application/octet-stream");
        }
        catch
        {
            await stream.DisposeAsync();
            throw;
        }
    }
}