using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using BepInEx.Unity.Mono.Bootstrap;
using HarmonyLib;
using RDLevelEditor;
using RhythmFramework;
using RhythmFramework.Events;
using RhythmFramework.Patches;
using RhythmFramework.Interfaces;
using RhythmFramework.Options;
using RhythmFramework.Options.Enum;
using RhythmFramework.Options.Patches;

[assembly: AssemblyFileVersion(RhythmFrameworkPlugin.VersionString)]
[assembly: AssemblyVersion(RhythmFrameworkPlugin.VersionString)]
[assembly: AssemblyInformationalVersion(RhythmFrameworkPlugin.VersionString)]
namespace RhythmFramework;

/// <summary>
/// Rhythm Framework's main class.
/// </summary>
[BepInPlugin(Id, "RhythmFramework", VersionString)]
[BepInProcess("Rhythm Doctor.exe")]
public class RhythmFrameworkPlugin : BaseUnityPlugin
{
    /// <summary>
    /// The version of the plugin.
    /// </summary>
    public const string VersionString = MajorVersion + "." + MinorVersion + "." + PatchVersion;

    /// <summary>
    /// Major number of the plugin (1st Number).
    /// </summary>
    public const string MajorVersion = "0";
    /// <summary>
    /// Minor number of the plugin (2nd Number).
    /// </summary>
    public const string MinorVersion = "1";
    /// <summary>
    /// Patch number of the plugin (3rd Number).
    /// </summary>
    public const string PatchVersion = "0";
    
    /// <summary>
    /// Id of the plugin. Proba
    /// </summary>
    public const string Id = "com.rhythmframework.api";

    private static Harmony _harmony;
    
    /// <summary>
    /// Instance of the RhythmFramework plugin.
    /// </summary>
    public static RhythmFrameworkPlugin Instance = null!;
    
    internal static Assembly _assembly = null!;
    internal new static ManualLogSource Logger;

    private Dictionary<Assembly, IRhythmPlugin> assemblyToPluginMap = null!;

    /// <summary>
    /// Get the <see cref="IRhythmPlugin"/> out of an <see cref="Assembly"/>.
    /// </summary>
    /// <param name="assembly">The assembly to get the <see cref="IRhythmPlugin"/> for.</param>
    /// <param name="plugin">The plugin instance.</param>
    /// <returns>A <see cref="IRhythmPlugin"/> instance or null.</returns>
    public bool TryGetRhythmPlugin(Assembly assembly, out IRhythmPlugin plugin)
    {
        plugin = assemblyToPluginMap.TryGetValue(assembly, out IRhythmPlugin value) ? value : null!;
        return plugin != null;
    }

    private void Awake()
    {
        Instance = this;
        Logger = base.Logger;
        _assembly = Assembly.GetExecutingAssembly();

        // Awake code here, setup matters
        ForceDevPatch.Enabled = true;
        assemblyToPluginMap = new Dictionary<Assembly, IRhythmPlugin>();
        OptionController.Initialize();

        _harmony = new Harmony(Id);
        _harmony.PatchAll(_assembly);
        
        #if DEBUG
        CustomEventController.Register(Assembly);
        CustomMethod.RegisterCustomMethods(Assembly);
        #endif
        UnityChainloader.Instance.PluginLoaded += OnPluginLoaded;

        Logger.LogInfo($"RD Project is loaded!");
    }

    private void OnPluginLoaded(PluginInfo info)
    {
        if (info.Instance is not IRhythmPlugin rhythmPlugin) return; // Only interpret plugins 
        Logger.LogInfo($"Registering Plugin {info.Metadata.Name}...");
        Assembly? assembly = null;
        try
        {
            // Get the type of the plugin instance and retrieve its assembly
            assembly = info.Instance.GetType().Assembly;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to get assembly from plugin instance: {ex.Message}");
        }
        if (assembly == null) return;
        assemblyToPluginMap.Add(assembly, rhythmPlugin);
        
        CustomEventController.Register(assembly);
        CustomMethod.RegisterCustomMethods(assembly);
        
        Logger.LogInfo($"Loaded Plugin {info.Metadata.GUID} ({rhythmPlugin.ModID}) with Rhythm Framework.");
    }
}