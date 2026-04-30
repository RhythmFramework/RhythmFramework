using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HarmonyLib;
using RDLevelEditor;
using RhythmFramework.Events;

namespace RhythmFramework.Patches.Editor;

// ReSharper disable InconsistentNaming
static class RDLevelDataPatch
{
    [HarmonyPatch(typeof(RDLevelData), nameof(RDLevelData.Encode)), HarmonyPrefix]
    public static void EncodePrefixPatch(RDLevelData __instance)
    {
        if (!__instance.levelEvents.Any(e => e is CustomEvent)) return; // Don't add our IDs if they don't have custom ones.
        
        List<LevelEvent_Base> moddedEvents = __instance.levelEvents.Where(e => e is CustomEvent).ToList();
        List<string> modIds = [];
        
        moddedEvents.ForEach(e =>
        {
            string outputId = RhythmFrameworkPlugin.Instance.TryGetRhythmPlugin(e.GetType().Assembly, out var plugin)
                ? $"{ModConstants.ModName}_{plugin.ModID}" // Outside custom event
                : ModConstants.ModName; // Built in event
            if (!modIds.Contains(outputId)) modIds.Add(outputId);
        });
        if (!modIds.Any()) return;
        
        __instance.settings.mods ??= [];
        List<string> currentMods = __instance.settings.mods.ToList();
        currentMods.AddRange(modIds);
        __instance.settings.mods = currentMods.Distinct().ToArray(); // Remove duplicates.
    }
}