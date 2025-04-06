using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RhythmFramework.Extensions;
using RhythmFramework.Options.Enum;
using RhythmFramework.Options.Game;
using RhythmFramework.Utilities;

namespace RhythmFramework.Options.Patches;

[HarmonyPatch(typeof(PauseMenuData), nameof(PauseMenuData.GetModeContents))]
internal static class GetModeContentsPatch
{
    public static void Postfix(PauseMenuData __instance, PauseModeName modeName, ref List<PauseMenuContentData> __result)
    {
        if (__result == null) return;
        PauseMenuContentData backButton = __result.PopLast();
        bool addBack = backButton.name == PauseContentName.Back;
        if (!addBack) __result.Add(backButton);
        foreach (var kvp in OptionController.CategoryOptions)
        {
            OptionCategory category = kvp.Key;
            bool canAdd = false;
            canAdd = canAdd || category is OptionCategory.GameAndMenu or OptionCategory.Game &&
                modeName is PauseModeName.GameSettings;
            canAdd = canAdd || category is OptionCategory.GameAndMenu or OptionCategory.Menu &&
                modeName is PauseModeName.MainMenuSettings;
            canAdd = canAdd || category is OptionCategory.Audio && modeName is PauseModeName.AudioSettings;
            canAdd = canAdd || category is OptionCategory.Accessibility && modeName is PauseModeName.AccessibilitySettings;
            canAdd = canAdd || category is OptionCategory.Advanced && modeName is PauseModeName.AdvancedSettings;
            
            if (canAdd) AddOptionsToCategory(kvp.Value, ref __result);
        }
        // Add the `Back Button` back to the end of the list so it looks nice.
        if (addBack) __result.Add(backButton);
    }

    private static void AddOptionsToCategory(List<GameOption> addedOptions, ref List<PauseMenuContentData> rdOptionList)
    {
        foreach (var opt in addedOptions)
        {
            if (opt == null) continue;
            string key = opt.Key();
            CustomPauseContentData optionData = new(opt)
            {
                name = EnumExtensions.ExtendEnum<PauseContentName>(string.Concat(key.Split(' ').Select(word => char.ToUpper(word[0]) + word.Substring(1)))),
                valueType = EnumExtensions.GetFromExtendedName<PauseContentValueType>("Custom"),
                stringSelectablesValue = opt.Values.Select(v => v.GetText()).ToArray(),
                currentIndex = opt.DefaultIndex
            };
            rdOptionList.Add(optionData);  
        }
    }
}