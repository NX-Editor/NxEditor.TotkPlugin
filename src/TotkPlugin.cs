using CsRestbl;
using Native.IO.Services;
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
        NativeLibraryManager.RegisterAssembly(typeof(TotkPlugin).Assembly, out bool isCommonLoaded)
            .Register(new RestblLibrary(), out bool isRestblLoaded);

        Console.WriteLine($"Loaded native_io: {isCommonLoaded}");
        Console.WriteLine($"Loaded cs_retbl: {isRestblLoaded}");

        TotkConfig.SetRestblStrings(TotkConfig.Shared.RestblGameVersion);
        Frontend.Locate<IMenuFactory>().Append(new TotkActionsMenu());

        serviceManager
            .Register(new TotkZstd());
    }
}
