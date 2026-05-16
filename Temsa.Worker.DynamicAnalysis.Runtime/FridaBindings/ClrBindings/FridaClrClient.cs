using System.Collections.Concurrent;
using Frida;
using Microsoft.Extensions.Logging;

namespace Temsa.Worker.DynamicAnalysis.Runtime.FridaBindings.ClrBindings;

public class FridaClrClient(
    ILogger<FridaClrClient> logger) : IFridaClient
{
    private readonly ILogger<FridaClrClient> _logger = logger;

    public Task<IFridaSession> SpawnAsync(
        FridaSpawnOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.TargetIdentifier);

        var deviceManager = new DeviceManager();
        var device = ResolveDevice(deviceManager, options.DeviceId);
        
        _logger.LogInformation(
            "Spawning target {TargetIdentifier} on Frida device {DeviceId}",
            options.TargetIdentifier,
            device.Id);

        var pid = device.Spawn(
            options.TargetIdentifier,
            null,
            null,
            null,
            null);

        var session = device.Attach(pid);

        var scripts = new List<Script>();
        var messages = new BlockingCollection<FridaMessage>();

        foreach (var scriptDefinition in options.Scripts)
        {
            var script = session.CreateScript(scriptDefinition.Source);

            script.Message += (_, args) =>
            {
                messages.Add(new FridaMessage(
                    ScriptId: scriptDefinition.Id,
                    Message: args.Message,
                    Data: args.Data));
            };
            
            script.Load();
            scripts.Add(script);
            
            _logger.LogInformation("Loaded Frida script {ScriptId}", scriptDefinition.Id);
        }
        
        device.Resume(pid);
        
        _logger.LogInformation("Resumed target {TargetIdentifier}, pid {pid}",
            options.TargetIdentifier,
            pid);

        IFridaSession fridaSession = new FridaClrSession(
            session,
            scripts,
            messages,
            logger: _logger);

        return Task.FromResult(fridaSession);
    }

    private Device ResolveDevice(
        DeviceManager deviceManager,
        string? deviceId)
    {
        var devices = deviceManager.EnumerateDevices();
        
        _logger.LogDebug(
            "Frida devices: {Devices}",
            string.Join(", ", devices.Select(x => $"{x.Id}/{x.Name}/{x.Type}")));

        if (!string.IsNullOrWhiteSpace(deviceId))
        {
            var deviceById = devices.FirstOrDefault(x =>
                string.Equals(x.Id, deviceId, StringComparison.OrdinalIgnoreCase));

            if (deviceById is null)
            {
                throw new InvalidOperationException(
                    $"Frida device with id '{deviceId}' was not found. Available devices: " +
                    string.Join(", ", devices.Select(x => $"{x.Id}/{x.Name}/{x.Type}")));
            }

            return deviceById;
        }
        
        var usbDevice = devices.FirstOrDefault(x =>
            string.Equals(x.Type.ToString(), "Usb", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.Type.ToString(), "USB", StringComparison.OrdinalIgnoreCase));
        
        if (usbDevice is not null)
        {
            return usbDevice;
        }
        
        var remoteDevice = devices.FirstOrDefault(x =>
            string.Equals(x.Type.ToString(), "Remote", StringComparison.OrdinalIgnoreCase));

        if (remoteDevice is not null)
        {
            return remoteDevice;
        }

        throw new InvalidOperationException(
            "No suitable Frida device was found. Available devices: " +
            string.Join(", ", devices.Select(x => $"{x.Id}/{x.Name}/{x.Type}")));
    }
}