using Temsa.Contracts.Artifacts;

namespace Temsa.Core.Application.Projects.Commands.UploadProjectArtifact;

public record UploadProjectArtifactCommand(
    long ProjectId,
    ProjectArtifactType Type,
    ArtifactKind Kind,
    Stream Content,
    string FileName,
    string? ContentType);