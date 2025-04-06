using HarmonyLib;
using RhythmFramework.Extensions;
using RhythmFramework.Options.Game;
using RhythmFramework.Utilities;

namespace RhythmFramework.Options.Patches;

[HarmonyPatch(typeof(PauseMenuContentData))]
internal static class PauseMenuContentDataPatch
{
    private static PauseContentValueType CustomName => EnumExtensions.GetFromExtendedName<PauseContentValueType>("Custom");
    
    [HarmonyPatch(typeof(PauseMenuContentData), nameof(PauseMenuContentData.CanChangeLeft)), HarmonyPrefix]
    public static bool CanChangeLeftPatch(PauseMenuContentData __instance, ref bool __result)
    {
        if (__instance.valueType != CustomName) return true;
        __result = true;
        return false;
    }
    [HarmonyPatch(typeof(PauseMenuContentData), nameof(PauseMenuContentData.CanChangeRight)), HarmonyPrefix]
    public static bool CanChangeRightPatch(PauseMenuContentData __instance, ref bool __result)
    {
        if (__instance.valueType != CustomName) return true;
        __result = true;
        return false;
    }
    [HarmonyPatch(typeof(PauseMenuContentData), nameof(PauseMenuContentData.ChangeValue)), HarmonyPrefix]
    public static bool ChangeValuePatch(PauseMenuContentData __instance, int direction, bool shiftPressed, ref string __result)
    {
        if (__instance.valueType != CustomName) return true;
        RDBypasses.BypassPrivateVariable(typeof(PauseMenuContentData), "changedValueUsingShift").SetValue(__instance, shiftPressed);
        int newIndex = EnforceIndexConstraint(__instance.currentIndex + direction, __instance.stringSelectablesValue.Length);
        __instance.changedValue = __instance.currentIndex != newIndex;
        __instance.currentIndex = newIndex;
        __result = __instance.stringSelectablesValue[newIndex];
        return false;
    }
    [HarmonyPatch(typeof(PauseMenuContentData), nameof(PauseMenuContentData.SetStartValue)), HarmonyPrefix]
    public static bool SetStartValuePatch(PauseMenuContentData __instance, object value, ref string __result)
    {
        if (__instance.valueType != CustomName) return true;
        int newIndex = EnforceIndexConstraint((int)value, __instance.stringSelectablesValue.Length);
        __instance.currentIndex = newIndex;
        __result = __instance.stringSelectablesValue[newIndex];
        return false; 
    }
    
    private static int EnforceIndexConstraint(int index, int listLength)
    {
        if (index >= listLength)
            return 0;
        if (index < 0)
            return listLength - 1;
        return index;
    }
}