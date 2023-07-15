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
        StatusModal.Set(Encoding.UTF8.GetString(handle.Data[0..6]));
        return handle.Data.AsSpan()[0..6].SequenceEqual("RESTBL"u8);
    }
}
