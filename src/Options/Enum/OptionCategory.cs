namespace RhythmFramework.Options.Enum;

/// <summary>
/// An enum denoting what category this option should fall under.
/// </summary>
public enum OptionCategory
{
    /// <summary>
    /// This option will only appear in game.
    /// In game includes the Story Mode level select.
    /// Appears under General.
    /// </summary>
    Game,
    /// <summary>
    /// This option will only appear in the main menu.
    /// Does not include the Story Mode level select.
    /// Appears under General.
    /// </summary>
    Menu,
    /// <summary>
    /// This option will appear in game and on the main menu.
    /// Appears under General.
    /// </summary>
    GameAndMenu,
    /// <summary>
    /// This option will appear under Advanced regardless of where the settings are.
    /// </summary>
    Advanced,
    /// <summary>
    /// This option will appear under Audio regardless of where the settings are.
    /// Audio settings are regenerated every time the player switches to them.
    /// </summary>
    Audio,
    /// <summary>
    /// This option will appear under Accessibility regardless of where the settings are.
    /// </summary>
    Accessibility
}