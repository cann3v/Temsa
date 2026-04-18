namespace Temsa.Core.Application.Abstractions.Time;

public class SystemDateTimeProvider: IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
