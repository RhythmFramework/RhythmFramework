using System.Collections;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmFramework.Events.SetWindowTitle;

[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
// ReSharper disable once InconsistentNaming
class LevelEvent_SetWindowTitle: CustomEvent
{
    public override KeyCode Shortcut() => KeyCode.P;

    public override EventCategory Category() => EventCategory.Utility;
    
    public override IEnumerator Prepare()
    {
        yield break;
    }
    
    public override void Run()
    {
        RhythmFrameworkPlugin.Logger.LogInfo($"Setting window title {WindowTitle}");
    }

    public override string GetTooltipText() => WindowTitle;

    [JsonProperty("", "", null, "", false, true, null)]
    [InputField(null, InputField.LineType.SingleLine, 100, 14, true, true, null)]
    public string WindowTitle { get; set; } = "";
}