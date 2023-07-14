using CommunityToolkit.Mvvm.ComponentModel;
using ConfigFactory.Core;
using ConfigFactory.Core.Attributes;

namespace NxEditor.TotkPlugin;

public partial class TotkConfig : ConfigModule<TotkConfig>
{
    public override string Name { get; } = "totk";

    [ObservableProperty]
    [property: Config(
        Header = "Game Path",
        Description = """
        The absolute path to your TotK RomFS game dump
        (e.g. F:\Games\Totk\RomFS)

        Required Files:
        - 'Pack/ZsDic.pack.zs'
        """,
        Category = "TotK")]
    [property: BrowserConfig(
        BrowserMode = BrowserMode.OpenFolder,
        InstanceBrowserKey = "totk-config-game-path",
        Title = "TotK RomFS Game Path")]
    private string _gamePath = string.Empty;

    [ObservableProperty]
    [property: Config(
        Header = "zStd Compression Level",
        Description = "Compression level used when compressing with zStd\n(Restart required)",
        Category = "TotK")]
    [property: DropdownConfig(
        RuntimeItemsSourceMethodName = "GetCompressionLevels"
    )]
    private string _zstdCompressionLevel = "16";

    partial void OnGamePathChanged(string value)
    {
        SetValidation(() => GamePath, value => {
            return value is not null
                && File.Exists(Path.Combine(value, "Pack", "ZsDic.pack.zs"));
        });
    }

    public static string[] GetCompressionLevels()
    {
        string[] result = new string[22];
        for (int i = 0; i < 22; i++) {
            result[i] = (i + 1).ToString();
        }

        return result;
    }
}
