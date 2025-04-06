# Rhythm Framework

## Information:
A modding framework for Rhythm Doctor.

Features:
- Simple binding of functions to the **Call Custom Method** event
  - You attach a `CustomMethod` attribute to a static function and the framework will handle the rest.
- Create custom **events** in the editor
  - These will work exactly like the original game if done correctly. Look at the example project. 
  - If someone downloads your level and doesn't have a mod that adds an event, they won't be able to play the level.
- Add new options seamlessly to Rhythm Doctor's Pause Menu
- Some new utilities/extensions

# Getting Started
Resources for using the framework:
1. [Installation Setup](#installation)
2. [Project Setup](#project-setup)
3. [Custom Events](#custom-events)
4. [Custom Methods](#custom-methods)
5. [Creating New Options](#creating-options)

## Installation
Whether you are installing RhythmFramework to use with a mod, or developing with it, the process is the same.
<br/>Make sure these are downloaded and installed in a folder before continuing:
- [Rhythm Doctor](https://rhythmdr.com/) - Base Game
- [BepInEx #692](https://builds.bepinex.dev/projects/bepinex_be/692/BepInEx-Unity.Mono-win-x64-6.0.0-be.692%2B851521c.zip) - Required for C# code Injection (without this, the game isn't modded)

Next, you'll need to download RhythmFramework, which can be found in the **Releases** tab.<br/>
<ins>Mod Authors</ins> should provide the version in their download. But versions are usually cross-compatible so try the latest if confused.

Once downloaded, move **RhythmFramework.dll** over to `BepInEx/plugins` in your RD folder.<br/>
Once its there, you're good to go!

If you are a developer, make sure to add a reference by DLL in your csproj.

## Project Setup
This assumes you already have some experience in modding unity games.

Add the `IRhythmPlugin` interface to your plugin class.
```csharp
using RhythmFramework.Interfaces;

[BepInPlugin(Id, "MyRDMod", VersionString)]
[BepInProcess("Rhythm Doctor.exe")]
public class MyRDMod : BaseUnityPlugin, IRhythmPlugin
```
<br/>
In your classes' body, add a `ModID` property with a get only.

```csharp
public string ModID => "ExampleID";
```
The ID is not used currently, however it will be soon.<br/>
It will be used for Custom Events and will be added to a level's mods to denote that it uses custom events from a mod.<br/>
As custom events currently aren't finished, it has no use.

## Custom Events
<ins>**Before continuing**, please note that custom events are not currently finished. Proceed with caution.</ins>

For this example, we will be creating an event called `SetWindowTitle`.<br/>
Custom events need two classes, we will cover the actual event class first.

### Level Event
Create a class that inherits `CustomEvent`. Make sure it starts with `LevelEvent_`. Implement any missing variables or functions.<br/>
```csharp
public class LevelEvent_SetWindowTitle: CustomEvent
{
    public override KeyCode Shortcut() => KeyCode.P;

    public override EventCategory Category() => EventCategory.Utility;
}
```
<br><br/>
Add the LevelEventInfo attribute to your event class. This is needed for RD to parse your event.
```csharp
[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
// ReSharper disable once InconsistentNaming
public class LevelEvent_SetWindowTitle: CustomEvent
```
<br><br/>
Override these functions:
```csharp
// "Prepares" the event. Basically do all the calculations and everything important here.
public override IEnumerator Prepare()
{
    yield break;
}

// Called when the event should actually happen. So on the bar and beat it is set to in the editor.
public override void Run()
{
    RhythmFrameworkPlugin.Logger.LogInfo($"Setting window title {WindowTitle}");
}

// This text shows up when you hover over the event for some time.
public override string GetTooltipText() => WindowTitle;
```
<br><br/>
Next, we need to add the values we want to save to the json for our event.
To do this, you need to add `JsonProperty` attribute. It tells RD to export this value when saving your level.
```csharp
[JsonProperty("", "", null, "", false, true, null)]
[InputField(null, InputField.LineType.SingleLine, 100, 14, true, true, null)]
public string WindowTitle { get; set; } = "";
```
**Note**: You <ins>**need**</ins> `{ get; set; }` as these attributes only work on properties.<br><br/>
`InputField` tells RD that this property is a textbox. There are a lot of other attributes to denote what type of value it is like:
- BeatModifers
- BPMCalculator
- Button
- Checkbox
- Color
- Description
- DontShow
- Dropdown
- ExpPositionPicker
- FlipScreen
- Hand
- Image
- InputField
- PulsePicker
- ReorderRooms
- SetRoomPerspective
- ShowRooms
- Slider
- SliderAlpha
- SliderPercent
- Sound
- ToggleGroup

You can pair them with an `InfoAttribute` to narrow it down even further:
- ColorInfo
- ConditionalInfo
- Float2Info
- FloatInfo
- IntInfo
- SoundDataInfo
- Vector2Info
- Float2PropertyInfo

(i don't think these do anything, but just in case, use them.)

If everything was done correctly, this side of the event should be done.
You can build the game and you should see your custom event in the events section.

### Inspector Panel
Create a new class that inherits `InspectorPanel`. Make sure it starts with `InspectorPanel_` and is named the same as the event class and same namespace.
```csharp
public class InspectorPanel_SetWindowTitle: InspectorPanel
```
<br><br/>
And that's all you really need, the framework should handle the rest using your attributes.
You can still add stuff to your function.

This is a `MonoBehaviour`, so you can create an Awake Call and call base.
```csharp
private new void Awake()
{
    base.Awake();
}
```
<br><br/>
You can also override these functions for some more control. They are pretty self-explanatory.
```csharp
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
```

## Custom Methods
Custom methods are pretty simple to set up.<br><br/>
However, they must be a static method and can only use the 4 basic types (string, int, float, bool).

Here is an example of a custom event:
```csharp
[CustomMethod]
private static void ExamplePrint(float value)
{
    Logger.LogInfo(value);
}
```
You would call it in RD by just typing `ExamplePrint(1.5)`. The framework will handle everything else for you.

## Creating Options
Custom options are also pretty easy to use.<br><br/>
You use `OptionController` to register them.
```csharp
OptionController.RegisterOption(new GameOptionBuilder()
    .Name("Example Option")
    .Description("Example description")
    .Category(OptionCategory.Accessibility)
    .AddBoolean()
    .BindBool(b => Logger.LogDebug($"printing value: {b}"))
    .Build());
```
The framework will handle everything else for you.

There are 6 categories:
```csharp
    // Only appears "IN-GAME", meaning you cannot find this option on the main menu where the logo is.
    // "IN-GAME" also includes the story mode settings menu.
    // Appears under General.
    Game, 
        
    // Only appears "in main menu", so you can only find this by pressing space on settings on the title screen.
    // Appears under General.
    Menu,
    
    // Allows you to have the option appear on both "IN-GAME" and "in main menu".
    // Will appear under General for both cases.
    GameAndMenu,
    
    // Pretty self-explanatory. Appears under Advanced regardless of where the settings are.
    Advanced,
    
    // Pretty self-explanatory. Appears under Audio regardless of where the settings are.
    // You might encounter some issues adding options under here.
    // I can't seem to replicate them all the time so proceed with caution.
    Audio,
    
    // Pretty self-explanatory. Appears under Accessibility regardless of where the settings are.
    Accessibility
```
