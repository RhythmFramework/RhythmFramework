using RDLevelEditor;
using UnityEngine.UI;

namespace RhythmFramework.Events.SetWindowTitle;

// ReSharper disable InconsistentNaming
class InspectorPanel_SetWindowTitle: InspectorPanel
{
    public override string ToString() => "Set Window Title";

    private new void Awake()
    {
        base.Awake();
        base.AddOnEditListeners(null, [
            windowTitle
        ]);
    }

    public override void UpdateUIInternal(LevelEvent_Base levelEvent)
    {
        LevelEvent_SetWindowTitle levelEvent_SetWindowTitle = (LevelEvent_SetWindowTitle)levelEvent;
        this.windowTitle.text = levelEvent_SetWindowTitle.WindowTitle;
    }

    protected override void SaveInternal(LevelEvent_Base levelEvent)
    {
        LevelEvent_SetWindowTitle levelEvent_SetWindowTitle = (LevelEvent_SetWindowTitle)levelEvent;
		levelEvent_SetWindowTitle.WindowTitle = this.windowTitle.text;
    }

    public InputField windowTitle;
}