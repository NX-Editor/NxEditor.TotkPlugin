using Avalonia.Controls;
using NxEditor.PluginBase.Models;
using NxEditor.TotkPlugin.ViewModels;

namespace NxEditor.TotkPlugin.Demo.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        RestblEditorViewModel vm = new(new FileHandle("D:\\Bin\\RSTB\\totk\\ResourceSizeTable.Product.111.rsizetable"));
        vm.Read();
        Src.Content = vm.View;
    }
}