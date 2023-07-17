using NxEditor.PluginBase;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;
using NxEditor.TotkPlugin.ViewModels;
using System.Text;

namespace NxEditor.TotkPlugin.Providers;
internal class RestblEditorProvider : IFormatServiceProvider
{
    public IFormatService GetService(IFileHandle handle)
    {
        return new RestblEditorViewModel(handle);
    }

    public bool IsValid(IFileHandle handle)
    {
        return handle.Data.AsSpan()[0..6].SequenceEqual("RESTBL"u8);
    }
}
