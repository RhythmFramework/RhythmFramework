using System.Collections.Generic;
using RhythmFramework.Extensions;
using RhythmFramework.Options.Enum;
using RhythmFramework.Options.Game;

namespace RhythmFramework.Options;

/// <summary>
/// A controller for handling custom options.
/// </summary>
public static class OptionController
{
    internal static readonly Dictionary<OptionCategory, List<GameOption>> CategoryOptions = new();

    internal static void Initialize()
    {
        EnumExtensions.ExtendEnum<PauseContentValueType>("Custom");
    }

    /// <summary>
    /// Register an option to RD's pause menu.
    /// </summary>
    /// <param name="option">The option to register.</param>
    public static void RegisterOption(GameOption option)
    {
        if (CategoryOptions.TryGetValue(option.AppearingMode, out List<GameOption>? list)) list.Add(option);
        else CategoryOptions[option.AppearingMode] = [option];
    }

    /// <summary>
    /// Register a list of options to RD's pause menu.
    /// </summary>
    /// <param name="options">The options to register.</param>
    public static void RegisterOptions(IEnumerable<GameOption> options) => options.ForEach(RegisterOption);
    /// <summary>
    /// Register a list of options to RD's pause menu.
    /// </summary>
    /// <param name="options">The options to register.</param>
    public static void RegisterOptions(params GameOption[] options) => options.ForEach(RegisterOption);
}