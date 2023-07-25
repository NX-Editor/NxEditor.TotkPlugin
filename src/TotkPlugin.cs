using NxEditor.PluginBase;
using NxEditor.TotkPlugin.Providers;

namespace NxEditor.TotkPlugin;

public class TotkPlugin : IServiceExtension
{
    public string Name { get; } = "NxEditor.TotkPlugin";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        serviceManager
            .Register(new TotkZstd())
            .Register("RestblEditor", new RestblEditorProvider());
    }
}
