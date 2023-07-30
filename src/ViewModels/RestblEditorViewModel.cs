using AvaloniaEdit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsRestbl;
using NxEditor.PluginBase.Components;
using NxEditor.PluginBase.Models;
using NxEditor.TotkPlugin.Helpers;
using NxEditor.TotkPlugin.Models;
using NxEditor.TotkPlugin.Views;
using System.Collections.ObjectModel;
using System.Text;

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

    [RelayCommand]
    public void FormatText(TextEditor editor)
    {
        StringBuilder sb = new();

        foreach (var rawText in editor.Text.Replace("\r\n", "\n").Split('\n')) {
            string text = rawText.Trim();

            if (string.IsNullOrEmpty(text)) {
                sb.AppendLine();
                continue;
            }

            if (text.StartsWith('+') || text.StartsWith('*') || text.StartsWith('-') || text.StartsWith('#')) {
                sb.AppendLine(text);
                continue;
            }

            int index;
            string stringKey = (index = text.IndexOf(' ')) > -1 ? text[..index] : text;
            uint size = 0;

            if (_restbl.NameTable.Contains(stringKey)) {
                size = _restbl.NameTable[stringKey];
                sb.Append("* ");
                goto End;
            }

            uint hashKey = Crc32.Compute(stringKey);
            if (_restbl.CrcTable.Contains(hashKey)) {
                size = _restbl.CrcTable[hashKey];
                sb.Append("* ");
                goto End;
            }

            sb.Append("+ ");

        End:
            sb.Append(stringKey);
            sb.AppendLine($" = {size}");
        }

        editor.Text = sb.ToString();
    }
}
