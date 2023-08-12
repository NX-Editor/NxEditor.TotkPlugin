using NxEditor.PluginBase;
using NxEditor.PluginBase.Attributes;
using NxEditor.PluginBase.Common;
using NxEditor.PluginBase.Components;
using NxEditor.PluginBase.Models;
using System.Reflection;

namespace NxEditor.TotkPlugin.Models;

public class TotkActionsMenu
{
    private static readonly string _vanillaRestblPath = Path.Combine(TotkConfig.Shared.GamePath, "System", "Resource", "ResourceSizeTable.Product.120.rsizetable.zs");

    [Menu("Reset to Vanilla", "Totk/RSTB", icon: "fa-arrow-rotate-left")]
    public static async Task ResetToVanilla()
    {
        DialogResult result = await DialogBox.ShowAsync("Warning", """
             Changes made to the open RESTBL will be lost.
         
             If you don't have one already, consider generating
             a changeog (RCL) before resetting.
             """, secondaryButtonContent: "Cancel");

        if (result == DialogResult.Primary) {
            IEditor? current = Frontend.Locate<IEditorManager>().Current;
            if (current?.GetType() is Type type && type.Name == "RestblEditorViewModel") {
                type.GetMethod("ResetFromHandle", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(current, new object[] {
                    new FileHandle(_vanillaRestblPath)
                });
            }
        }
    }
}
