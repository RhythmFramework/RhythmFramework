using System;
using HarmonyLib;
using RDLevelEditor;
using RhythmFramework.Events;

namespace RhythmFramework.Events.Patches;

static class LevelEventBasePatch
{
    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(LevelEventControl_Base), nameof(LevelEventControl_Base.ShowDataOnInspector))]
    // public static bool ShowDataOnInspectorPrefix(LevelEvent_Base __instance)
    // {
    //     if (!__instance.GetType().IsSubclassOf(typeof(CustomEvent)))
    //         return true;

    //     RhythmFrameworkPlugin.Logger.LogInfo("custom showing event info");
    //     return false;
    // }
    
    [HarmonyPatch(typeof(LevelEvent_Base), nameof(LevelEvent_Base.inspectorPanel), MethodType.Getter)]
    public static class InspectorPanelPatch
    {
        public static bool Prefix(LevelEvent_Base __instance, ref InspectorPanel __result)
        {
            // RhythmFrameworkPlugin.Logger.LogInfo($"Typeof Custom Event: {__instance.GetType().IsSubclassOf(typeof(CustomEvent))}");
            if (!__instance.GetType().IsSubclassOf(typeof(CustomEvent))) return true;
            RhythmFrameworkPlugin.Logger.LogInfo("spoofing finish level inspector");
            __result = __instance.editor.inspectorPanelManager.Get(Type.GetType("RDLevelEditor.InspectorPanel_FinishLevel"));
            return false;
        }
    }

}