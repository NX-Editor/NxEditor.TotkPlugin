using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NxEditor.TotkPlugin.Demo;
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data?.GetType().FullName?.Replace("ViewModel", "View") is string name)
        {
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
        }

        return new TextBlock { Text = $"View for '{data?.GetType().FullName}' not found" };
    }

    public bool Match(object? data)
    {
        return data is ObservableObject;
    }
}