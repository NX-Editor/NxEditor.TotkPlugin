using NxEditor.PluginBase;

namespace NxEditor.TotkPlugin;

public class TotkPlugin : IServiceExtension
{
    public static string Name { get; } = "NxEditor.TotkPlugin";
    string IServiceExtension.Name => Name;

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        TotkConfig.SetRestblStrings(TotkConfig.Shared.RestblGameVersion);

        serviceManager
            .Register(new TotkZstd());
    }
}
