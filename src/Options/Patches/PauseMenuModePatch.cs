using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RhythmFramework.Extensions;
using RhythmFramework.Options.Game;
using UnityEngine.UI;

namespace RhythmFramework.Options.Patches;

[HarmonyPatch(typeof(PauseMenuMode))]
internal static class PauseMenuModePatch
{
    [HarmonyPatch(typeof(PauseMenuMode), nameof(PauseMenuMode.Show)), HarmonyPostfix]
    public static void ShowPatch(PauseMenuMode __instance)
    {
        foreach (KeyValuePair<Text, string> keyValuePair in __instance.textsKeysDict)
        {
            Text key = keyValuePair.Key;
            if (key == null) continue;
            if (key != __instance.titleText)
            {
                string outputText = key.text;
                if (outputText.Contains("no key: "))
                {
                    string ending = outputText.Substring("no key: pauseMenu.".Length);
                    if (int.TryParse(ending, out int enumNum))
                    {
                        var targetName = (PauseContentName)enumNum;
                        PauseMenuContentData? matchingData = __instance.categories
                            .SelectMany(c => c.contentsDataList)
                            .FirstOrDefault(c => c.name == targetName);
                        if (matchingData is CustomPauseContentData customData)
                        {
                            key.text = outputText = customData.BaseOption.Name();
                        }
                    }
                }
    
                key.text = outputText;
            }
            bool flag = key.cachedTextGenerator.lineCount >= 3;
            key.lineSpacing = flag ? 0.8f : 1f;
            PauseMenuMode.CheckCJKText(key);
        }
    }
    
    // For some extremely odd reason, the game specifically regenerates the AudioSettings category every time you switch to it.
    // I'm not a dev, so I couldn't tell you why, but it's extremely odd.
    // There's probably a better solution to whatever issue they had to program that.
    [HarmonyPatch(typeof(PauseMenuMode), nameof(PauseMenuMode.ChangeContentValue)), Harmony]
    public static void ChangeContentPostfix(PauseMenuMode __instance)
    {
        if (__instance.CurrentCategory.modeName != PauseModeName.AudioSettings) return;
        ShowPatch(__instance); // just recheck all text. copy and paste would make code too messy.
    }
    //
    // [HarmonyPatch(typeof(PauseMenuMode), nameof(PauseMenuMode.Initialize)), HarmonyPostfix]
    // public static void InitializePostfix(PauseMenuMode __instance, RDPauseMenu mainPauseMenu, PauseMenuModeData modeData, PauseMenuData pauseMenuData)
    // {
    //     ShowPatch(__instance);
    // }
}