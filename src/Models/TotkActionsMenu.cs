﻿using ConfigFactory.Avalonia.Helpers;
using ConfigFactory.Core.Attributes;
using NxEditor.PluginBase;
using NxEditor.PluginBase.Attributes;
using NxEditor.PluginBase.Common;
using NxEditor.PluginBase.Components;
using NxEditor.PluginBase.Models;
using System.Reflection;
using TotkRstbGenerator.Core;

namespace NxEditor.TotkPlugin.Models;

public class TotkActionsMenu
{
    private static readonly string _vanillaRestblPath = Path.Combine(TotkConfig.Shared.GamePath, "System", "Resource", "ResourceSizeTable.Product.120.rsizetable.zs");

    [Menu("Reset to Vanilla", "Totk/RSTB", icon: "fa-arrow-rotate-left")]
    public static async Task ResetToVanilla()
    {
        await CallActionMethod("ResetFromHandle", async () => {
            return await DialogBox.ShowAsync("Warning", """
                Changes made to the open RESTBL will be lost.
         
                If you don't have one already, consider generating
                a changeog (RCL) before resetting.
                """, secondaryButtonContent: "Cancel") == DialogResult.Primary;
        });
    }

    [Menu("Generate Changelog (RCL)", "Totk/RSTB", icon: "fa-wand-magic-sparkles", IsSeparator = true)]
    public static async Task GenerateRCL()
    {
        await CallActionMethod("GenerateRclFromHandle");
    }

    [Menu("Calculate RSTB", "Totk/RSTB", icon: "fa-percent")]
    public static async Task CalculateRstb()
    {
        BrowserDialog dialog = new(BrowserMode.OpenFolder, "Open Mod Folder (RomFS)");
        if (await dialog.ShowDialog() is string path && path.EndsWith("romfs", StringComparison.InvariantCultureIgnoreCase)) {
            RstbGenerator generator = new(path);
            await generator.GenerateAsync();

            await DialogBox.ShowAsync("Generated RSTB", $"""
                RSTB successfully generated in '{path}'
                """);
        }
    }

    private static async Task CallActionMethod(string methodName, Func<Task<bool>>? condition = null)
    {
        IEditor? current = Frontend.Locate<IEditorManager>().Current;
        if (current?.GetType() is Type type && type.Name == "RestblEditorViewModel") {
            if (condition == null || condition.Invoke() is Task<bool> task && await task == true) {
                type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(current, [
                    EditorFile.FromFile(_vanillaRestblPath)
                ]);
            }

            return;
        }
        
        StatusModal.Set("Current document is not a restbl file", "fa-solid fa-triangle-exclamation", isWorkingStatus: false, temporaryStatusTime: 1.7);
    }
}
