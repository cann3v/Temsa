namespace Temsa.Common.Files;

public class TemporaryDirectory : IAsyncDisposable, IDisposable
{
    public string Path { get; }

    private bool _disposed;

    private TemporaryDirectory(string path)
    {
        Path = path;
    }

    public static TemporaryDirectory Create(string prefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);

        var path = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            $"{prefix.TrimEnd('-', '_')}-{Guid.NewGuid():N}");

        Directory.CreateDirectory(path);

        return new TemporaryDirectory(path);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        TryDelete(Path);
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch { }
    }
}
