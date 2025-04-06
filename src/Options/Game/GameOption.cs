using RhythmFramework.Options.Enum;

namespace RhythmFramework.Options.Game;

/// <summary>
/// An option that appears in the game's settings menu.
/// </summary>
public class GameOption: Option
{
    /// <summary>
    /// The category this opton will appear under.
    /// </summary>
    public OptionCategory AppearingMode = OptionCategory.GameAndMenu;
}