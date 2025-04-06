namespace RhythmFramework.Interfaces;

/// <summary>
/// An interface that tells the framework your mod uses this plugin.
/// </summary>
public interface IRhythmPlugin
{
    /// <summary>
    /// The string here is added the level's mods if you use any of their custom methods.
    /// Please make it short but sweet as this will be displayed to other modded players when they play a level and don't have your events.
    /// </summary>
    string ModID { get; }
}