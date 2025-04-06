namespace RhythmFramework.Options.Game;

internal class CustomPauseContentData(GameOption baseOption): PauseMenuContentData
{
    public GameOption BaseOption { get; } = baseOption;
}