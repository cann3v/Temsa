namespace Temsa.Common.Time;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
