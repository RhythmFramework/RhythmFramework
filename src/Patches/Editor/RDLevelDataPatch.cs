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
    [HarmonyPatch(typeof(RDLevelData), nameof(RDLevelData.Decode)), HarmonyPrefix]
    public static bool DecodePatch(Dictionary<string, object> rootDict, bool onlyActiveEvents, bool onlySettings, ref RDLevelData __result)
    {
        // version param is completely useless.
        RDLevelData rdlevelData = new RDLevelData(new RDLevelSettings(61), null, null, new List<Conditional>(), null, new List<BookmarkData>(), new string[21]) {
            errorName = LevelErrorName.None,
            levelEvents =  new List<LevelEvent_Base>(),
            rows = new List<LevelEvent_MakeRow>(),
            sprites = new List<LevelEvent_MakeSprite>(),
            conditionals = new List<Conditional>(),
            bookmarks = new List<BookmarkData>(),
            colorPalette = new string[21]
        };
        RDLevelData.current = rdlevelData;
        if (rootDict.ContainsKey("settings"))
        {
            try
            {
                Dictionary<string, object> dict = (rootDict["settings"] as Dictionary<string, object>)!;
                rdlevelData.settings.Decode(dict);
                goto IL_D4;
            }
            catch
            {
                RhythmFrameworkPlugin.Logger.LogWarning("Error loading RDLevelSettings");
                RDLevelData.decodingFailed = true;
                rdlevelData.errorName = LevelErrorName.LevelSettingsNull;
                __result = rdlevelData;
                return false;
            }
        }
        RhythmFrameworkPlugin.Logger.LogDebug("THIS IS r1??? Calling setup.");
        rdlevelData.settings = new RDLevelSettings(1);
        IL_D4:
        foreach (object obj in (rootDict["rows"] as List<object>)!)
        {
            Dictionary<string, object> dict2 = (Dictionary<string, object>)obj;
            LevelEvent_MakeRow levelEventMakeRow = new LevelEvent_MakeRow();
            levelEventMakeRow.Decode(dict2);
            rdlevelData.levelEvents.Add(levelEventMakeRow);
            rdlevelData.rows.Add(levelEventMakeRow);
        }
        if (rootDict.TryGetValue("decorations", out object? decor))
        {
            foreach (object obj2 in (decor as List<object>)!)
            {
                Dictionary<string, object> dict3 = (Dictionary<string, object>)obj2;
                LevelEvent_MakeSprite levelEventMakeSprite = new LevelEvent_MakeSprite();
                levelEventMakeSprite.Decode(dict3);
                rdlevelData.levelEvents.Add(levelEventMakeSprite);
                rdlevelData.sprites.Add(levelEventMakeSprite);
            }
        }
        InspectorPanel_SetVFXPreset.legacyPresetsInLevel.Clear();
        WindowDanceController.changingWindowSize = false;
        foreach (object obj3 in (rootDict["events"] as List<object>)!)
        {
            Dictionary<string, object> dictionary = (Dictionary<string, object>)obj3;
            string str = "";
            foreach (string text in dictionary.Keys)
            {
                str += $"{text}: {dictionary[text]} | ";
            }

            string str2 = (dictionary["type"] as string)!;

            Type? type = CustomEventController.EventTypeFromName(str2); // Try to find custom event first.
            if (type == null) type = Type.GetType("RDLevelEditor.LevelEvent_" + str2); // If can't, search vanilla events.
            if (type == null || !type.IsSubclassOf(typeof(LevelEvent_Base)))
            {
                RhythmFrameworkPlugin.Logger.LogWarning("Could not find event named: " + str2);
            }
            else
            {
                LevelEvent_Base levelEventBase = (LevelEvent_Base)Activator.CreateInstance(type);
                levelEventBase.Decode(dictionary);
                if (levelEventBase.usesWindowDance)
                {
                    rdlevelData.settings.usesWindowDance = true;
                }
                if (levelEventBase is LevelEvent_WindowResize)
                {
                    rdlevelData.settings.resizesWindow = true;
                }
                if (levelEventBase.active || !onlyActiveEvents)
                {
                    rdlevelData.levelEvents.Add(levelEventBase);
                    if (levelEventBase is LevelEvent_PlaySong && onlySettings)
                    {
                        RDLevelData.decodingFailed = false;
                        __result = rdlevelData;
                        return false;
                    }
                }
            }
        }
        if (rootDict.TryGetValue("conditionals", out object? conditions))
        {
            List<object> list = (conditions as List<object>)!;
            ImmutableDictionary<string, ConditionalInfo> conditionalsInfo = GC.conditionalsInfo;
            foreach (object obj4 in list)
            {
                Dictionary<string, object> dict4 = (Dictionary<string, object>)obj4;
                try
                {
                    rdlevelData.conditionals.Add(Conditional.Decode(dict4));
                }
                catch (Exception exception)
                {
                    RhythmFrameworkPlugin.Logger.LogWarning("Error adding conditional, see following exception:");
                    RhythmFrameworkPlugin.Logger.LogError(exception);
                    RDLevelData.decodingFailed = true;
                    rdlevelData.errorName = LevelErrorName.LevelDataNull;
                    __result = rdlevelData;
                    return false;
                }
            }
        }
        if (rootDict.TryGetValue("bookmarks", out object? bookmarks))
        {
            foreach (object obj5 in (bookmarks as List<object>)!)
            {
                Dictionary<string, object> dictionary2 = (Dictionary<string, object>)obj5;
                try
                {
                    BarAndBeat newBarAndBeat = new BarAndBeat((int)dictionary2["bar"], Convert.ToSingle(dictionary2["beat"]));
                    BookmarkData item = new BookmarkData(newBarAndBeat, (int)dictionary2["color"]);
                    rdlevelData.bookmarks.Add(item);
                }
                catch (Exception ex)
                {
                    RhythmFrameworkPlugin.Logger.LogError("Error adding bookmarks.");
                    RhythmFrameworkPlugin.Logger.LogError(ex);
                    RDLevelData.decodingFailed = true;
                    rdlevelData.errorName = LevelErrorName.LevelEventsNull;
                    __result = rdlevelData;
                    return false;
                }
            }
        }
        if (rootDict.TryGetValue("colorPalette", out object? palette))
        {
            List<object> list2 = (palette as List<object>)!;
            int num = 0;
            using List<object>.Enumerator enumerator = list2.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object? obj6 = enumerator.Current;
                string dictValue = (string)obj6;
                try
                {
                    rdlevelData.colorPalette[num] = RDEditorUtils.DecodeColor(dictValue, true);
                    num++;
                    if (num >= 21)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    RhythmFrameworkPlugin.Logger.LogError("Error adding color palette colors.");
                    RhythmFrameworkPlugin.Logger.LogError(ex);
                    RDLevelData.decodingFailed = true;
                    rdlevelData.errorName = LevelErrorName.LevelEventsNull;
                    __result = rdlevelData;
                    return false;
                }
            }
            goto IL_560;
        }
        rdlevelData.colorPalette = RDEditorConstants.defaultColorPalette;
        IL_560:
        RDBaseDllDummy.printes("version is " + rdlevelData.settings.version.ToString());
        string[] mods = rdlevelData.settings.mods;
        var list3 = mods == null ? [] : mods.ToList();
        if (rdlevelData.settings.version < 47) list3.Add("booleansDefaultToTrue");
        if (rdlevelData.settings.version < 49) list3.Add("classicHitParticles");
        if (rdlevelData.settings.version < 53) list3.Add("legacyTaggedEvents");
        
        rdlevelData.settings.mods = list3.ToArray();
        (from o in rdlevelData.levelEvents
            orderby o.sortOrder
            select o).ToList<LevelEvent_Base>();
        RDLevelData.decodingFailed = false;
        __result = rdlevelData;
        return false;
    }

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