using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using RDLevelEditor;
using RhythmFramework.Utilities;
using RhythmFramework.Events;
using RhythmFramework.Events.Attributes;

namespace RhythmFramework.Events.Patches;

[HarmonyPatch(typeof(LevelEvent_CallCustomMethod))]
static class CallCustomMethodPatch
{
    [HarmonyPatch(typeof(LevelEvent_CallCustomMethod), nameof(LevelEvent_CallCustomMethod.Prepare)), HarmonyPostfix]
    public static void PreparePostfix(LevelEvent_CallCustomMethod __instance, ref System.Collections.IEnumerator __result)
    {
        __result = PatchedPrepare(__instance);
    }

    public static IEnumerator PatchedPrepare(LevelEvent_CallCustomMethod @event)
    {
        @event.methodName = @event.game.currentLevel.EvaluateCurlyBracketsInString(@event.methodName);
		@event.executionTime = @event.levelEventExecution;
		@event.sortOrderOffset = @event.sortOffset;
		Type? type = LevelEvent_Base.level.GetType();
		Type? type2 = null;
		if (@event.methodName.StartsWith("vfx."))
		{
			@event.methodName = @event.methodName.Remove(0, 4);
			type2 = typeof(scrVfxControl);
			RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "instance").SetValue(@event, @event.vfx);
		}
		else if (@event.methodName.StartsWith("level."))
		{
			@event.methodName = @event.methodName.Remove(0, 6);
		}
		else if (@event.methodName.StartsWith("room"))
		{
			if (@event.methodName.StartsWith("room[0].") || @event.methodName.StartsWith("room[1].") || @event.methodName.StartsWith("room[2].") || @event.methodName.StartsWith("room[3]."))
			{
				int num = (int)(@event.methodName[5] - '0');
				@event.methodName = @event.methodName.Remove(0, 8);
				type2 = typeof(RDRoom);
				RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "instance").SetValue(@event, @event.game.rooms[num]);
			}
			else if (@event.methodName.StartsWith("room1.") || @event.methodName.StartsWith("room2.") || @event.methodName.StartsWith("room3.") || @event.methodName.StartsWith("room4."))
			{
				int num2 = (int)(@event.methodName[4] - '0');
				@event.methodName = @event.methodName.Remove(0, 6);
				type2 = typeof(RDRoom);
				RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "instance").SetValue(@event, @event.game.rooms[num2 - 1]);
			}
		}
		bool flag = @event.methodName.IndexOf("(", StringComparison.Ordinal) != -1 && @event.methodName.IndexOf("=", StringComparison.Ordinal) == -1;
		FieldInfo instance = RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "instance");
		FieldInfo method = RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "method");
		FieldInfo field = RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "field");
		Type operationType = RDBypasses.BypassPrivateEnum(typeof(LevelEvent_CallCustomMethod), "Operation");
		if (!flag)
		{
			int num3 = @event.methodName.IndexOf("=", StringComparison.Ordinal);
			if (num3 != -1)
				RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "operation").SetValue(@event, Enum.Parse(operationType, "Set"));
			else
			{
				num3 = @event.methodName.IndexOf("++", StringComparison.Ordinal);
				if (num3 != -1)
				{
					RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "operation").SetValue(@event, Enum.Parse(operationType, "Increment"));
				}
				else
				{
					num3 = @event.methodName.IndexOf("--", StringComparison.Ordinal);
					RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "operation").SetValue(@event, Enum.Parse(operationType, "Decrement"));
				}
			}
			if (num3 == -1)
			{
				RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "operation").SetValue(@event, Enum.Parse(operationType, "Get"));
			}
			string name = ((num3 != -1) ? @event.methodName.Substring(0, num3).Trim() : @event.methodName.Trim());
			if (RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "operation").GetValue(@event) == Enum.Parse(operationType, "Set"))
			{
				RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "strArg")
					.SetValue(@event, @event.methodName.Substring(num3 + 1, @event.methodName.Length - num3 - 1).Trim());
			}
			if (type2 != null) field.SetValue(@event, type2.GetField(name));
			
			if (field.GetValue(@event) == null) field.SetValue(@event, type.GetField(name));
			if (field.GetValue(@event) != null && instance.GetValue(@event) == null) instance.SetValue(@event, LevelEvent_Base.level);
			
			// Switch to methods if we cant find anything
			if (field.GetValue(@event) == null) flag = true;
		}
		if (flag)
		{
			string name2 = @event.methodName;
			string? text = null;
			bool flag2 = false;
			int num3 = @event.methodName.IndexOf("(", StringComparison.Ordinal);
			if (num3 > 0)
			{
				name2 = @event.methodName.Substring(0, num3).Trim();
				text = @event.methodName.Substring(num3, @event.methodName.Length - num3);
				flag2 = true;
			}
			if (type2 != null) method.SetValue(@event, type2.GetMethod(name2));
			
			if (method.GetValue(@event) == null && type != null) method.SetValue(@event, type.GetMethod(name2));
			if (method.GetValue(@event) != null && instance.GetValue(@event) == null) instance.SetValue(@event, LevelEvent_Base.level);
			
			if (method.GetValue(@event) == null)
			{
				// Finally check custom events.
				if (CustomMethod.RegisteredMethods.TryGetValue(name2, out var methodInfo))
				{
					// Method exists.
					MethodInvocation methodInvocation = methodInfo.Item2.Invocation;
					if (methodInvocation is MethodInvocation.Both 
					    || methodInvocation is MethodInvocation.OnlyPreBar && @event.levelEventExecution is LevelEventExecutionTime.OnPrebar
					    || methodInvocation is MethodInvocation.OnlyOnBar && @event.levelEventExecution is LevelEventExecutionTime.OnBar
					    )
					{
						method.SetValue(@event, methodInfo.Item1);
						instance.SetValue(@event, null);
					}
				}
			}
			
			if (flag2)
			{
				FieldInfo argString = RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "argString");
				argString.SetValue(@event, text.GetParameters());
				if (argString.GetValue(@event) != null)
					RDBypasses.BypassPrivateVariable(typeof(LevelEvent_CallCustomMethod), "hasArguments").SetValue(@event, true);
			}
			if (method.GetValue(@event) == null)
			{
				RhythmFrameworkPlugin.Logger.LogWarning("CallCustomMethod: Method " + @event.methodName + " doesn't exist");
			}
		}
		@event.prepared = true;
		yield return null;
    }
}