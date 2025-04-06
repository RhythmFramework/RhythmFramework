using Discord;
using HarmonyLib;

namespace RhythmFramework.Patches.Discord;

[HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
class DiscordActivityManager
{
    public static void Prefix(ref Activity activity)
    {
        bool statesIsEmpty = string.IsNullOrEmpty(activity.State); //bottom text
        bool detailsIsEmpty = string.IsNullOrEmpty(activity.Details); // top text

        if (ForceDevPatch.Enabled)
            activity.Details = detailsIsEmpty ? "Dev Build" : activity.Details + " (Dev Build)";
    }       
}