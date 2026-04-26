using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Temsa.Common.Configuration;

namespace Temsa.Common.RabbitMq;

public class RabbitMqConnection(IOptions<RabbitMqConnectionOptions> options, ILogger<RabbitMqConnection> logger)
    : IRabbitMqConnection
{
    private readonly RabbitMqConnectionOptions _options = options.Value;
    private readonly ILogger<RabbitMqConnection> _logger = logger;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    
    private IConnection? _connection;
    private bool _disposed;

    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_connection is { IsOpen: true })
        {
            return _connection;
        }
        
        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            if (_connection is not null)
            {
                await SafeDisposeConnectionAsync(_connection);
                _connection = null;
            }

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };

            _logger.LogDebug("Opening RabbitMQ connection to {Host}:{Port}", _options.Host, _options.Port);

            _connection = await factory.CreateConnectionAsync(cancellationToken);

            _logger.LogDebug("RabbitMQ connection opened successfully");

            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        
        await _connectionLock.WaitAsync();
        try
        {
            if (_connection is not null)
            {
                await SafeDisposeConnectionAsync(_connection);
                _connection = null;
            }
        }
        finally
        {
            _connectionLock.Release();
            _connectionLock.Dispose();
        }
    }

    private static async Task SafeDisposeConnectionAsync(IConnection connection)
    {
        try
        {
            if (connection.IsOpen)
            {
                await connection.CloseAsync();
            }
        }
        catch
        {
            // Игнорирование ошибок отключения
        }

        await connection.DisposeAsync();
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
