using CommunityToolkit.Mvvm.ComponentModel;
using NxEditor.PluginBase.Components;
using NxEditor.PluginBase.Models;
using NxEditor.TotkPlugin.Views;

namespace NxEditor.TotkPlugin.ViewModels;

public partial class RestblEditorViewModel : Editor<RestblEditorViewModel, RestblEditorView>
{
    public RestblEditorViewModel(IFileHandle handle) : base(handle) { }

    public override string[] ExportExtensions { get; } = { "RESTBL:*.rsizetable|", "zStd:*.rsizetable.zs|" };

    [ObservableProperty]
    private string _random = "Some Field";

    public override Task Read()
    {
        Random = "Some Read Field";
        return Task.CompletedTask;
    }

    public override Task<IFileHandle> Write()
    {
        throw new NotImplementedException();
    }
}
