using System.Collections.Immutable;
using HarmonyLib;
using RDLevelEditor;
using System;
using System.Reflection;
using System.Linq;
using MonoMod.Utils;
using RhythmFramework.Extensions;
using RhythmFramework.Utilities;
using RhythmFramework.Events;

namespace RhythmFramework.Patches;

[HarmonyPatch(typeof(RDStartup), nameof(RDStartup.Setup))]
static class StartupPatch
{
    private static bool shouldSetup;
    public static void Prefix()
    {
        shouldSetup = !RDStartup.hasInitialized;
    }
    public static void Postfix() // RDStartup is static so no need to use __instance.
    {
        if (!shouldSetup) return;
        shouldSetup = false;
        ImmutableDictionary<LevelEventType, LevelEventInfo> levelEventInfos = GC.levelEventsInfo.ToImmutableDictionary();
        RhythmFrameworkPlugin.Logger.LogInfo($"Loading Info for {CustomEventController.CustomEvents.Count} events.");
        CustomEventController.CustomEvents.ForEach(kvp =>
        {
            LevelEventInfo eventInfo = new(kvp.Value);
            levelEventInfos = levelEventInfos.Add(eventInfo.eventTypeEnum, eventInfo);
        });

        GC.levelEventsInfo = levelEventInfos;
    }
}

[HarmonyPatch(typeof(LevelEventInfo))]
[HarmonyPatch(MethodType.Constructor)]
[HarmonyPatch([typeof(Type)])]
static class LevelEventInfoPatch
{
    public static bool Prefix(LevelEventInfo __instance, Type eventType)
    {
        if (!CustomEventController.CustomEvents.ContainsValue(eventType)) return true;
        RhythmFrameworkPlugin.Logger.LogInfo($"Parsing event type {eventType.Name}.");
        RDBypasses.EditReadonlyField(__instance, nameof(LevelEventInfo.attribute), eventType.GetCustomAttribute<LevelEventInfoAttribute>());
        if (__instance.attribute == null)
            throw new Exception($"Event '{eventType} does not have an 'LevelEventInfoAttribute' attribute.");
        string eventName = CustomEventController.CustomEvents.GetKeyByValue(eventType);
        RDBypasses.EditReadonlyField(__instance, nameof(LevelEventInfo.eventTypeEnum), EnumExtensions.ExtendEnum<LevelEventType>(eventName));
        RDBypasses.EditReadonlyField(__instance, nameof(LevelEventInfo.propertiesInfo), (from fieldInfo in eventType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
            where fieldInfo.IsDefined(typeof(JsonPropertyAttribute))
            orderby fieldInfo.MetadataToken
            select BasePropertyInfo.FromProperty(fieldInfo, null)).ToImmutableList<BasePropertyInfo>());
        
        return false;
    }
}