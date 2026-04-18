namespace Temsa.Core.Application.Abstractions.Time;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
