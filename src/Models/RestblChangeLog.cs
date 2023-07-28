using CommunityToolkit.Mvvm.ComponentModel;

namespace NxEditor.TotkPlugin.Models;

public partial class RestblChangeLog : ObservableObject
{
    private static readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nx-editor", "resources", "rcl");

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _filePath;

    [ObservableProperty]
    private string _content;

    [ObservableProperty]
    private bool _isEnabled = false;

    public RestblChangeLog(string path)
    {
        _filePath = path;
        _name = Path.GetFileNameWithoutExtension(path);
        _content = File.ReadAllText(path);
    }

    public static List<RestblChangeLog> FromLocalStorage()
    {
        if (!Directory.Exists(_path)) {
            return new();
        }

        return Directory.EnumerateFiles(_path, "*.rcl")
            .Select(x => new RestblChangeLog(x))
            .ToList();
    }

    partial void OnNameChanged(string value)
    {
        File.Move(FilePath, FilePath = Path.Combine(_path, $"{value}.rcl"));
    }
}
