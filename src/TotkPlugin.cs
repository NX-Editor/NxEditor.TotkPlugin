using NxEditor.PluginBase;

namespace NxEditor.TotkPlugin;

public class TotkPlugin : IServiceExtension
{
    public string Name { get; } = "NxEditor.TotkPlugin";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        serviceManager
            .Register(new TotkZstd());
    }
}
