using CsRestbl;
using Native.IO.Services;
using NxEditor.PluginBase;
using NxEditor.TotkPlugin.Providers;

namespace NxEditor.TotkPlugin;

public class TotkPlugin : IServiceExtension
{
    public string Name { get; } = "NxEditor.TotkPlugin";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        NativeLibraryManager.RegisterPath("D:\\Bin\\native", out _)
            .Register(new RestblLibrary(), out _);

        serviceManager
            .Register(new TotkZstd())
            .Register("RestblEditor", new RestblEditorProvider());
    }
}
