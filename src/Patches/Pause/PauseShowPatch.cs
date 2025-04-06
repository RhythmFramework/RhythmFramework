using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;

namespace RhythmFramework.Patches.Pause;

[HarmonyPatch(typeof(RDPauseMenu), nameof(RDPauseMenu.Show))]
static class PauseShowPatch
{
    public static void Postfix(RDPauseMenu __instance)
    {
        // remove scan lines on phone
        // makes the UI 10x cleaner and nicer,
        __instance.transform.Find("Phone UI/Scanlines16Opacity").gameObject.SetActive(false);
    }
}