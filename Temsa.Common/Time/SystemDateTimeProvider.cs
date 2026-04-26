namespace Temsa.Common.Time;

public class SystemDateTimeProvider: IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
