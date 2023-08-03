using ConfigFactory.Core;
using ConfigFactory.Models;
using NxEditor.PluginBase;

namespace NxEditor.TotkPlugin;

public class TotkPlugin : IServiceExtension
{
    public string Name { get; } = "NxEditor.TotkPlugin";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        if (Frontend.Locate<ConfigPageModel>().ConfigModules.TryGetValue("EpdConfig", out IConfigModule? module)) {
            module.Properties["RestblStrings"].Property.SetValue(module,
                Path.Combine(GlobalConfig.Shared.StorageFolder, "plugins", Name, "Resources", "Restbl", "string-table-1.2.0.txt"));
        }
            
        serviceManager
            .Register(new TotkZstd());
    }
}
