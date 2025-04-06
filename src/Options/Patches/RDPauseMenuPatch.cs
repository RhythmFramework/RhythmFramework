using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using RhythmFramework.Options.Game;

namespace RhythmFramework.Options.Patches;

// ReSharper disable once InconsistentNaming
[HarmonyPatch(typeof(RDPauseMenu))]
internal static class RDPauseMenuPatch
{
    [HarmonyPatch(typeof(RDPauseMenu), nameof(RDPauseMenu.UpdateLevelDetail)), HarmonyPostfix]
    public static void UpdateDetailPostfix(RDPauseMenu __instance, string description, string value, bool showConfirmationText)
    {
        if (!description.Contains("no key:")) return;
        
        string ending = description.Substring("no key: pauseMenu.levelDetail.".Length);
        if (!int.TryParse(ending, out var _)) return;
        if (__instance.currentMode.contentsDataList[__instance.currentMode.CurrentContentIndex] is not CustomPauseContentData contentData) return;
        __instance.detailDescription.text = contentData.BaseOption.Description ?? string.Empty;
    }
}