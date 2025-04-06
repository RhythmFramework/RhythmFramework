using RDLevelEditor;
using UnityEngine;

namespace RhythmFramework.Events;

/// <summary>
/// The base class for custom events.
/// </summary>
public abstract class CustomEvent: LevelEvent_Base
{
    /// <summary>
    /// The key needed to press to auto select this event in the inspector.
    /// </summary>
    /// <returns>A <see cref="KeyCode"/>.</returns>
    public virtual KeyCode Shortcut() => KeyCode.None;
    
    /// <summary>
    /// The category this event should fall under.
    /// </summary>
    /// <returns>An <see cref="EventCategory"/>.</returns>
    public abstract EventCategory Category();
}

/// <summary>
/// An enum holding all the categories.
/// </summary>
public enum EventCategory
{
    /// <summary>
    /// This event will appear under every category.
    /// </summary>
    All,
    /// <summary>
    /// This event will only appear under All and Gameplay.
    /// </summary>
    Gameplay,
    /// <summary>
    /// This event will only appear under All and Row FX.
    /// </summary>
    RowFX,
    /// <summary>
    /// This event will only appear under All and Text.
    /// </summary>
    Text,
    /// <summary>
    /// This event will only appear under All and Environment.
    /// </summary>
    Environment,
    /// <summary>
    /// This event will only appear under All and Cam FX.
    /// </summary>
    CamFX,
    /// <summary>
    /// This event will only appear under All and Visual FX.
    /// </summary>
    VisualFX,
    /// <summary>
    /// This event will only appear under All and Utility.
    /// </summary>
    Utility,
    /// <summary>
    /// This event will only appear under All and Song.
    /// </summary>
    Song,
    /// <summary>
    /// This event will only appear under All and Sounds.
    /// </summary>
    Sounds
}