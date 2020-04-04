using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.Sound;
using RimWorld;
using HarmonyLib;

namespace Firestarter
{
    [StaticConstructorOnStartup]
    class HarmonyPatches
    {
        private const string noFirewatcher_ModName = "No Firewatcher";
        private const string combatExtended_ModName = "Combat Extended";

        static HarmonyPatches()
        {
#if DEBUG
            HarmonyInstance.DEBUG = true;
#endif
            Harmony harmony = new Harmony("rimworld.whyisthat.firestarter.main");

            FirePatches.DoDefaultFirePatches(harmony);

            if (ModLister.AllInstalledMods.FirstOrDefault(m => m.Name == combatExtended_ModName)?.Active == true)
            {
                Log.Message("Firestarter: Combat Extended detected.");
                //FireArrowPatches.DoCEFireArrowPatches(harmony);
            }
            else
                FireArrowPatches.DoFireArrowPatches(harmony);

        }

    }
}
