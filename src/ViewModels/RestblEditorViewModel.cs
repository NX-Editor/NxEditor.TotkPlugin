using AvaloniaEdit;
using CommunityToolkit.Mvvm.ComponentModel;
using CsRestbl;
using NxEditor.PluginBase.Components;
using NxEditor.PluginBase.Models;
using NxEditor.TotkPlugin.Models;
using NxEditor.TotkPlugin.Views;
using System.Collections.ObjectModel;

namespace NxEditor.TotkPlugin.ViewModels;

// Values are loaded when adding an entry to a changelog
// Changelogs (rcl) are simple yaml diffs
// Main editor (here) displays current changelog
// Backend contains source restbl (for changelog gen) and saving
// 
// Edit RESTBL workflow
// - Copy vanilla RESTBL
// - Open in nxe
// - Locate files to change (or add custom yaml entry like =< v0.3.5)
// - Save changelog to local storage
// - Save RESTBL to src as merged

public partial class RestblEditorViewModel : Editor<RestblEditorViewModel, RestblEditorView>
{
    private static readonly List<RestblChangeLog> _staticChangelogFiles = RestblChangeLog.FromLocalStorage();
    private static readonly string _stringTable = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nx-editor", "resources", "string-table-1.2.0.txt");
    private bool _isChangeLocked = false;
    private Restbl _restbl = new();

    public RestblEditorViewModel(IFileHandle handle) : base(handle) { }

    public override string[] ExportExtensions { get; } = { "RESTBL:*.rsizetable|", "zStd:*.rsizetable.zs|" };

    [ObservableProperty]
    private ObservableCollection<RestblChangeLog> _changelogFiles = new(_staticChangelogFiles);

    [ObservableProperty]
    private RestblChangeLog? _current;

    public override Task Read()
    {
        View.StringsEditor.Text = File.ReadAllText(_stringTable);
        _restbl = Restbl.FromBinary(Handle.Data);

        View.TextEditor.TextChanged += (s, e) => {
            if (Current is not null && !_isChangeLocked) {
                Current.HasChanged = true;
            }

            _isChangeLocked = false;
        };

        return Task.CompletedTask;
    }

    public override Task<IFileHandle> Write()
    {
        throw new NotImplementedException();
    }

    partial void OnCurrentChanging(RestblChangeLog? oldValue, RestblChangeLog? newValue)
    {
        if (oldValue is not null) {
            oldValue.Content = View.TextEditor.Text;
        }
    }

    partial void OnCurrentChanged(RestblChangeLog? value)
    {
        _isChangeLocked = true;
        View.TextEditor.Text = value?.Content;
    }
}
