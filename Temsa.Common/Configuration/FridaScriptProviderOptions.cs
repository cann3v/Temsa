namespace Temsa.Common.Configuration;

public class FridaScriptProviderOptions
{
    public const string SectionName = "FridaScripts";

    public string RootDirectory { get; init; } = "scripts";
    public string ProfilesDirectory { get; init; } = "profiles";
}