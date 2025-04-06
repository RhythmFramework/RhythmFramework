using HarmonyLib;

namespace RhythmFramework.Patches;

/// <summary>
/// A class to force enable dev mode.
/// </summary>
public static class ForceDevPatch
{
    private static bool _enabled = false;
    
    /// <summary>
    /// Whether Dev Mode is enabled.
    /// </summary>
    public static bool Enabled
    {
        get => _enabled;
        set => CheckValue(value);
    }

    private static void CheckValue(bool value)
    {
        if (value) Enable();
        else Disable();
    }

    private static void Enable()
    {
        if (_enabled) return;
        _enabled = true;
        DebugSettings.instance.Debug = true;
    }

    private static void Disable()
    {
        if (!_enabled) return;
        _enabled = false;
        DebugSettings.instance.Debug = false;
    }

    [HarmonyPatch(typeof(RDBase), nameof(RDBase.isDev), MethodType.Getter), HarmonyPostfix]
    private static void DevCheckPostfix(ref bool __result)
    {
        if (!Enabled) return;
        __result = true;
    }
}

