using NxEditor.PluginBase;
using NxEditor.PluginBase.Components;
using NxEditor.TotkPlugin.Models;

namespace NxEditor.TotkPlugin;

public class TotkPlugin : IServiceExtension
{
    public static string Name { get; } = "NxEditor.TotkPlugin";
    string IServiceExtension.Name => Name;

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        TotkConfig.SetRestblStrings(TotkConfig.Shared.RestblGameVersion);
        Frontend.Locate<IMenuFactory>().Append(new TotkActionsMenu());

        serviceManager
            .Register(new TotkZstd());
    }
}
