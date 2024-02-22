using NxEditor.PluginBase;
using NxEditor.PluginBase.Components;
using NxEditor.TotkPlugin.Models;

namespace NxEditor.TotkPlugin;

public class TotkPlugin : IServiceExtension
{
    public string Name { get; } = "TotK Plugin";
    public string Version { get; } = typeof(TotkPlugin).Assembly.GetName().Version?.ToString(3) ?? "1.0.0";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        TotkConfig.SetRestblStrings(TotkConfig.Shared.RestblGameVersion);
        Frontend.Locate<IMenuFactory>().Append(new TotkActionsMenu());

        serviceManager
            .Register(new TotkZstd());
    }
}
