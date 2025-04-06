using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RDLevelEditor;
using RhythmFramework.Utilities;
using RhythmFramework.Events;

namespace RhythmFramework.Extensions;

/// <summary>
/// An extension class for <see cref="scnEditor"/>/
/// </summary>
// ReSharper disable once InconsistentNaming
internal static class scnEditorExtensions
{
    public static void SetLevelEventControlTypeForCustomEvents(this scnEditor editor, string name, bool copyRow = false)
    {
        Type? eventType = CustomEventController.EventTypeFromName(name);
        if (eventType == null)
        {
            // don't know how this would happen but, if it does, then here we go
            editor.SetLevelEventControlType(RDUtils.ParseEnum<LevelEventType>(name, LevelEventType.None), copyRow);
            return;
        }
        RhythmFrameworkPlugin.Logger.LogInfo($"trying to instantiate: {eventType.FullName}");
        if (editor.usingShowButton)
        {
            if (RDEditorUtils.ControlIsPressed())
            {
                SelectLevelEventPanel panel = (SelectLevelEventPanel)RDBypasses.BypassPrivateVariable(editor.GetType(), "actionEventPanel").GetValue(editor);
                panel.events.ForEach(delegate(SelectLevelEventButton levelEvent)
                {
                    levelEvent.selected = false;
                });
            }
            // editor.editor. Nice one RD devs.
            string withSpaces = name.WithSpaces();
            SelectLevelEventButton selectLevelEventButton = editor.editor.inspectorPanelManager.eventListPanels[(int)editor.currentTab].GetComponent<SelectLevelEventPanel>().events.Find((SelectLevelEventButton levelEvent) => levelEvent.label.text == withSpaces);
            selectLevelEventButton.selected = !selectLevelEventButton.selected;
            return;
        }
        LevelEvent_Base levelEvent2 = editor.selectedControl.levelEvent;
        LevelEvent_Base levelEvent_Base = (LevelEvent_Base)Activator.CreateInstance(eventType);
        levelEvent_Base.OnCreate();
        levelEvent_Base.CopyBasePropertiesFrom(levelEvent2, true, copyRow);
        RhythmFrameworkPlugin.Logger.LogDebug($"Is Subclass: {levelEvent_Base.GetType().IsSubclassOf(typeof(CustomEvent))}");
        Tab tab = editor.selectedControl.tab;
        if (tab == Tab.Sprites && levelEvent_Base.type != LevelEventType.None)
        {
            LevelEvent_MakeSprite levelEvent_MakeSprite = editor.editor.currentPageSpritesData[levelEvent_Base.y];
            levelEvent_Base.target = levelEvent_MakeSprite.spriteId;
        }
        editor.DeleteEventControl(editor.selectedControl, false, false);
        LevelEventControl_Base levelEventControl_Base = editor.CreateEventControl(levelEvent_Base, tab, false);
        levelEventControl_Base.UpdateUI();
        editor.UpdateTimelineAccordingToLevelEventType(LevelEventType.None);
        editor.SelectEventControl(levelEventControl_Base, false);
    }
}