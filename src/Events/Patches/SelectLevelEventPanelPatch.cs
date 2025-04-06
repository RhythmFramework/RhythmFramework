using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.UI;
using RhythmFramework.Events;
using RhythmFramework.Extensions;
using static RhythmFramework.Utilities.RDBypasses;

namespace RhythmFramework.Events.Patches;

[HarmonyPatch]
class SelectLevelEventPanelAwakePatch
{
    static MethodBase TargetMethod()
    {
        return typeof(SelectLevelEventPanel).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    static bool Prefix(SelectLevelEventPanel __instance)
    {
        if (__instance.willDisplayLevelEventsFromTab != Tab.Actions)
            return true;
        // Converting this to bypass was more annoying than anything. PLEASE USE PUBLIC VARIABLES. IM BEGGING YOU
        RhythmFrameworkPlugin.Logger.LogInfo("Running custom SelectLevelEventPanel...");
        var thisType = typeof(SelectLevelEventPanel);
        if (BypassPrivateVariable(thisType, "categoriesForTab", true).GetValue(null) == null)
        {
            BypassPrivateMethod(thisType, "CategoryDictInit").Invoke(__instance, []);
        }
        int num = 0;
        TabSection tabSection = __instance.editor.tabSections[(int)__instance.willDisplayLevelEventsFromTab];
        Color color = __instance.gc.colorPalette[(int)__instance.willDisplayLevelEventsFromTab];

        List<string> allEvents = tabSection.availableEvents.Select(x => x.ToString()).ToList();
        List<LevelEventType> eventTypes = new();

        CustomEventController.AddCustomEventsToList(allEvents);
        if (allEvents.Count > tabSection.shortcuts.Length)
        {
            RhythmFrameworkPlugin.Logger.LogInfo($"Adjusting shortcut count...");
            int additionalItemsNeeded = allEvents.Count - tabSection.shortcuts.Length;
            KeyCode[] newShortcuts = new KeyCode[allEvents.Count];
            Array.Copy(tabSection.shortcuts, newShortcuts, tabSection.shortcuts.Length);
            
            for (int i = tabSection.shortcuts.Length; i < newShortcuts.Length; i++)
            {
                newShortcuts[i] = KeyCode.None;
            }
            
            // RhythmFrameworkPlugin.Logger.LogInfo($"New shortcuts array length: {newShortcuts.Length}");
            // for (int i = 0; i < newShortcuts.Length; i++)
            // {
            //     RhythmFrameworkPlugin.Logger.LogInfo($"New shortcut[{i}]: {newShortcuts[i]}");
            // }

            tabSection.shortcuts = newShortcuts;
            // RhythmFrameworkPlugin.Logger.LogInfo($"Updated Shortcuts Count: {tabSection.shortcuts.Length}");
        }
        
        // Sort alphabetically. I always hated that it was just a garbled mess.
        var eventsWithShortcuts = new List<(string eventStr, KeyCode shortcut)>();
        for (int i = 0; i < allEvents.Count; i++)
        {
            eventsWithShortcuts.Add((allEvents[i], tabSection.shortcuts[i]));
        }

        eventsWithShortcuts.Sort((a, b) => string.CompareOrdinal(a.eventStr, b.eventStr));
        allEvents.Clear();
        KeyCode[] sortedShortcuts = new KeyCode[eventsWithShortcuts.Count];


        foreach (var (eventStr, shortcut) in eventsWithShortcuts)
        {
            allEvents.Add(eventStr);
            int index = allEvents.Count - 1;
            sortedShortcuts[index] = shortcut;
        }

        tabSection.shortcuts = sortedShortcuts;
        eventTypes.Clear();
        foreach (var eventStr in allEvents)
        {
            eventTypes.Add(RDUtils.ParseEnum<LevelEventType>(eventStr, LevelEventType.None));
        }

        // allEvents.Sort((a, b) => a.CompareTo(b));
        // allEvents.ForEach(e => eventTypes.Add(RDUtils.ParseEnum<LevelEventType>(e, LevelEventType.None)));
        RhythmFrameworkPlugin.Logger.LogInfo("c1");
        RhythmFrameworkPlugin.Logger.LogInfo($"Shortcuts Count: {tabSection.shortcuts.Length}");
        RhythmFrameworkPlugin.Logger.LogInfo($"eventTypes Count: {allEvents.Count}");
        RhythmFrameworkPlugin.Logger.LogInfo($"AllEvents Count: {allEvents.Count}");
        // for (int i = 0; i < tabSection.shortcuts.Length; i++)
        // {
        //     RhythmFrameworkPlugin.Logger.LogInfo($"New shortcut[{i}]: {tabSection.shortcuts[i]}");
        // }
        allEvents.ForEach((eventName, index) => {
            RhythmFrameworkPlugin.Logger.LogInfo($"{eventName} - {index}");
            LevelEventType type = eventTypes[index];
            // allow other hidden events if dev.
            // if (Array.IndexOf<LevelEventType>((LevelEventType[])BypassPrivateVariable(thisType, "hiddenEvents").GetValue(__instance), type) != -1 && (type != LevelEventType.WindowResize || !RDBase.isDev))
            if (Array.IndexOf((LevelEventType[])BypassPrivateVariable(thisType, "hiddenEvents").GetValue(__instance)!, type) != -1 && !RDBase.isDev)
            {
                num++;
            }
            else
            {
                SelectLevelEventButton component = UnityEngine.Object.Instantiate(__instance.selectLevelEventButtonPrefab).GetComponent<SelectLevelEventButton>();
                component.rect.SetParent(__instance.content, false);
                if (type == LevelEventType.None) component.label.text = eventName.WithSpaces();
                else component.label.text = RDString.Get("editor." + eventName);
                // component.label.text = "SUBSCRIBE TO DISCUSSIONS";
                RDStringToUIText.Apply(component.label, true, false, 0f);
                component.shortcut.text = tabSection.shortcuts[num].ToString().Replace("Alpha", "");
                component.shortcut.color = color;
                component.shortcutColor = color;
                component.icon.sprite = LevelEventControl_Base.GetIconFromName(eventName, false);
                component.button.onClick.AddListener(delegate()
                {
                    __instance.editor.conditionalsPanel.currentPanel = null;
                    bool copyRow = __instance.willDisplayLevelEventsFromTab == Tab.Sprites;
                    if (type == LevelEventType.None) __instance.editor.SetLevelEventControlTypeForCustomEvents(eventName, copyRow);
                    else __instance.editor.SetLevelEventControlType(type, copyRow);
                    __instance.LevelEditorPlaySound("sndEditorEventSetType", "LevelEditorParent", 1f, 1f, 0f);
                });
                component.type = type;
                __instance.events.Add(component);
                component.selected = true;
                num++;
            }
        });
        RhythmFrameworkPlugin.Logger.LogInfo("c2");
        var togglesEvents = BypassPrivateVariable(thisType, "togglesEvents");
        togglesEvents.SetValue(__instance, __instance.selectToggle != null && __instance.showText != null && __instance.showIcons != null);
        if ((bool)togglesEvents.GetValue(__instance))
        {
            BypassPrivateVariable(thisType, "selectToggleButton").SetValue(__instance, __instance.selectToggle!.GetComponent<Button>());
            BypassPrivateVariable(thisType, "selectToggleText").SetValue(__instance, __instance.selectToggle.GetComponentInChildren<Text>());
            BypassPrivateMethod(thisType, "UpdateSelectToggle").Invoke(__instance, []);
            __instance.lastSelectedEventsNumber = -1;
        }
        RhythmFrameworkPlugin.Logger.LogInfo("c3");
        var hasCategories = BypassPrivateVariable(thisType, "hasCategories");
        hasCategories.SetValue(__instance, false);

        // I tried way too long to get the dropdown working, and now I give up.
        // rhythm doctor and the stupid private stuff.

        if (__instance.categorySelect != null)
            __instance.categorySelect.SetActive(false);
        
        // if ((bool)hasCategories.GetValue(__instance))
        // {
        //     var categorySelectDropdown = BypassPrivateVariable(thisType, "categorySelectDropdown");
        //     Dropdown dropdownComponent = __instance.categorySelect.GetComponentInChildren<Dropdown>();
        //     categorySelectDropdown.SetValue(__instance, dropdownComponent);
        //     if (RDString.isCJK)
        //     {
        //         dropdownComponent.template.GetComponentInChildren<Text>().font = RDString.currentLanguageFont;
        //         dropdownComponent.captionText.font = RDString.currentLanguageFont;
        //     } 
            
        //     var categoriesForTabField = BypassPrivateVariable(thisType, "categoriesForTab", true);
        //     var categoryIdsField = BypassPrivateVariable(thisType, "categoryIds");
        //     var currentCategoryField = BypassPrivateVariable(thisType, "currentCategory");

        //     RhythmFrameworkPlugin.Logger.LogInfo("test 1");
        //     // Retrieve the values using reflection
        //     var categoriesForTab = (Dictionary<object, Array>)categoriesForTabField.GetValue(null);
        //     RhythmFrameworkPlugin.Logger.LogInfo("test 2");
        //     var selectedTab = __instance.willDisplayLevelEventsFromTab;
        //     var values = categoriesForTab[selectedTab];

        //     // Map enum values to Dropdown.OptionData for dropdown options and dictionary for value mapping
        //     List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        //     Dictionary<int, object> valueMapping = new Dictionary<int, object>();

        //     for (int i = 0; i < values.Length; i++)
        //     {
        //         var enumValue = values.GetValue(i);
        //         options.Add(new Dropdown.OptionData(enumValue.ToString()));
        //         valueMapping[i] = enumValue;
        //     }

        //     // Populate dropdown with options
        //     dropdownComponent.ClearOptions();
        //     dropdownComponent.AddOptions(options);

        //     // categoryIdsField.SetValue(__instance, valueMapping);

        //     dropdownComponent.onValueChanged.AddListener(delegate(int p0)
        //     {
        //         // var dict = (Dictionary<int, object>)categoryIds.GetValue(__instance);
        //         BypassPrivateVariable(thisType, "currentCategory").SetValue(__instance, valueMapping[dropdownComponent.value]);
        //         __instance.OnEnable();
        //     });
        // }
        RhythmFrameworkPlugin.Logger.LogInfo("c4");
        var hasGridViewButton = BypassPrivateVariable(thisType, "hasGridViewButton");

        // Disable grid view too since it is partially overlapped
        hasGridViewButton.SetValue(__instance, false);
        if (__instance.gridViewToggle != null)
            __instance.gridViewToggle.gameObject.SetActive(false);
        
        // hasGridViewButton.SetValue(__instance, __instance.gridViewToggle != null);
        // if ((bool)hasGridViewButton.GetValue(__instance))
        // {
        //     Button buttonComponent = __instance.gridViewToggle.GetComponent<Button>();
        //     BypassPrivateVariable(thisType, "gridViewButton").SetValue(__instance, buttonComponent);
        //     buttonComponent.onClick.AddListener(delegate()
        //     {
        //         var gridView = BypassPrivateVariable(thisType, "GridView");
        //         gridView.SetValue(__instance, !(bool)gridView.GetValue(__instance));
        //         __instance.OnEnable();
        //     });
        //     BypassPrivateVariable(thisType, "gridViewIcon").SetValue(__instance, __instance.gridViewToggle.GetComponentsInChildren<Image>().Last<Image>());
        //     __instance.UpdateGridViewToggle();
        // }
        RhythmFrameworkPlugin.Logger.LogInfo("c5");
        var hasRoomSelect = BypassPrivateVariable(thisType, "hasRoomSelect");
        hasRoomSelect.SetValue(__instance, hasRoomSelect.GetValue(__instance) != null);
        RhythmFrameworkPlugin.Logger.LogInfo("Ran custom SelectLevelEventPanel!");
        return false;
    }
}