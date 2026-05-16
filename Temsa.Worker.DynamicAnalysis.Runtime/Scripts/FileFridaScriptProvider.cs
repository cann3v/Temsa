using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Temsa.Common.Configuration;

namespace Temsa.Worker.DynamicAnalysis.Runtime.Scripts;

public class FileFridaScriptProvider(
    IHostEnvironment environment,
    IOptions<FridaScriptProviderOptions> options): IFridaScriptProvider
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IHostEnvironment _environment = environment;
    private readonly FridaScriptProviderOptions _options = options.Value;

    public async Task<IReadOnlyCollection<FridaLoadedScript>> LoadScriptAsync(
        string profileName,
        IReadOnlyCollection<string> enabledScripts,
        IReadOnlyCollection<string> disabledScripts,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(profileName);

        var rootPath = Path.Combine(
            _environment.ContentRootPath,
            _options.RootDirectory);

        var profilePath = Path.Combine(
            rootPath,
            _options.ProfilesDirectory,
            $"{profileName}.json");

        if (!File.Exists(profilePath))
        {
            throw new InvalidOperationException(
                $"Frida script profile '{profileName}' was not found at '{profilePath}'");
        }

        await using var profileStream = File.OpenRead(profilePath);

        var profile = await JsonSerializer.DeserializeAsync<FridaScriptProfile>(
            profileStream,
            JsonSerializerOptions,
            cancellationToken);

        if (profile is null)
        {
            throw new InvalidOperationException(
                $"Frida script profile '{profileName}' is empty or invalid");
        }

        var enabledSet = enabledScripts.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var disabledSet = disabledScripts.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var selectedScripts = profile.Scripts
            .Where(x => (x.Enabled || enabledSet.Contains(x.Id)) &&
                        !disabledSet.Contains(x.Id))
            .ToArray();

        var result = new List<FridaLoadedScript>();

        foreach (var script in selectedScripts)
        {
            var scriptPath = Path.Combine(rootPath, script.Path);

            if (!File.Exists(scriptPath))
            {
                throw new InvalidOperationException(
                    $"Frida script '{script.Id}' was not found at '{scriptPath}'");
            }

            var source = await File.ReadAllTextAsync(scriptPath, cancellationToken);
            result.Add(new FridaLoadedScript(
                script.Id,
                source));
        }

        return result;
    }
}