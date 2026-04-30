using System;
using System.Collections.Generic;
using HarmonyLib;
using RDLevelEditor;
using RhythmFramework.Events;
using UnityEngine;

namespace RhythmFramework.Patches.Editor;

[HarmonyPatch(typeof(RDLevelData), MethodType.Constructor, typeof(Dictionary<string, object>), typeof(bool), typeof(bool), typeof(bool))]
// ReSharper disable InconsistentNaming
static class RDLevelDataConstructorPatch
{
    public static void Postfix(RDLevelData __instance, Dictionary<string, object> rootDict, bool onlyActiveEvents, bool onlySettings, bool forceLatestVersion)
    {
        foreach (object obj3 in (rootDict["events"] as List<object>))
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)obj3;
			string str = "";
			foreach (string text in dictionary.Keys)
			{
				str += string.Format("{0}: {1} | ", text, dictionary[text]);
			}
			string str2 = dictionary["type"] as string;
			string text2 = "RDLevelEditor.LevelEvent_" + str2;
			Type? type = CustomEventController.EventTypeFromName(str2); // Try to find custom event first.
			if (type == null) type = Type.GetType("RDLevelEditor.LevelEvent_" + str2); // If can't, search vanilla events.
			if (type == null || !type.IsSubclassOf(typeof(LevelEvent_Base))) continue;
			LevelEvent_Base levelEvent_Base = (LevelEvent_Base)Activator.CreateInstance(type);
			levelEvent_Base.Decode(dictionary);
			if (levelEvent_Base.usesWindowDance && levelEvent_Base.active)
			{
				LevelEvent_NewWindowDance levelEvent_NewWindowDance = levelEvent_Base as LevelEvent_NewWindowDance;
				int num;
				if (levelEvent_NewWindowDance == null)
				{
					LevelEvent_WindowResize levelEvent_WindowResize = levelEvent_Base as LevelEvent_WindowResize;
					if (levelEvent_WindowResize == null)
					{
						LevelEvent_SetWindowContent levelEvent_SetWindowContent = levelEvent_Base as LevelEvent_SetWindowContent;
						if (levelEvent_SetWindowContent == null)
						{
							LevelEvent_RenameWindow levelEvent_RenameWindow = levelEvent_Base as LevelEvent_RenameWindow;
							if (levelEvent_RenameWindow == null)
							{
								LevelEvent_HideWindow levelEvent_HideWindow = levelEvent_Base as LevelEvent_HideWindow;
								num = ((levelEvent_HideWindow != null) ? levelEvent_HideWindow.window : 0);
							}
							else
							{
								num = levelEvent_RenameWindow.window;
							}
						}
						else
						{
							num = levelEvent_SetWindowContent.window;
						}
					}
					else
					{
						num = levelEvent_WindowResize.window;
					}
				}
				else
				{
					num = levelEvent_NewWindowDance.window;
				}
				var num2 = num;
				__instance.windowCount = Mathf.Max(__instance.windowCount, num2 + 1);
				__instance.usesWindowDance = true;
			}
			if (levelEvent_Base.active || !onlyActiveEvents)
			{
				__instance.levelEvents.Add(levelEvent_Base);
				if (levelEvent_Base is LevelEvent_PlaySong && onlySettings)
				{
					RDLevelData.decodingFailed = false;
					return;
				}
			}
		}
    }
}