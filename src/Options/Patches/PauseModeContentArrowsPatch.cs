using HarmonyLib;
using RhythmFramework.Extensions;
using RhythmFramework.Options.Game;

namespace RhythmFramework.Options.Patches;

[HarmonyPatch(typeof(PauseModeContentArrows))]
internal static class PauseModeContentArrowsPatch
{
    // WHY "7th Beat Games"
    // THIS IS SUCH TERRIBLE CODE
    // WHO HAD THE GREAT IDEA OF CODING SETTINGS LIKE THIS??
    
    [HarmonyPatch(typeof(PauseModeContentArrows), nameof(PauseModeContentArrows.UpdateValue)), HarmonyPrefix]
    public static void SetStartValuePrefix(PauseModeContentArrows __instance)
    {
        if (__instance.contentData.valueType == EnumExtensions.GetFromExtendedName<PauseContentValueType>("Custom"))
        {
            CustomPauseContentData contentData = (__instance.contentData as CustomPauseContentData)!;
            __instance.valueText.text = contentData.SetStartValue(contentData.BaseOption.DefaultIndex);
        }
    }

    [HarmonyPatch(typeof(PauseModeContentArrows), nameof(PauseModeContentArrows.ChangeContentValue)), HarmonyPrefix]
    public static void ChangeValuePrefix(PauseModeContentArrows __instance, int direction, bool shiftPressed)
    {
        if ((direction == 1 && !__instance.contentData.CanChangeRight()) || (direction == -1 && !__instance.contentData.CanChangeLeft())) return;
        if (!__instance.canChangeContentValue) return;
        
        if (__instance.contentData.valueType != EnumExtensions.GetFromExtendedName<PauseContentValueType>("Custom")) return;
        CustomPauseContentData contentData = (__instance.contentData as CustomPauseContentData)!;
        __instance.valueText.text = contentData.ChangeValue(direction, shiftPressed);
        if (contentData.changedValue) contentData.BaseOption.SetValue(contentData.currentIndex);
    }
}