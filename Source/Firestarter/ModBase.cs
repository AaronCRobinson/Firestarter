using System.Linq;
using UnityEngine.SceneManagement;
using Verse;
using Harmony;
using HugsLib;

namespace Firestarter
{
    // NOTE: consider removing need for HugsLib...
    public class FirestarterModBase : ModBase
    {
        private const string noFirewatcher_ModName = "No Firewatcher";
        private const string combatExtended_ModName = "Combat Extended";

        public override string ModIdentifier
        {
            get { return "Firestarter"; }
        }

        public override void DefsLoaded()
        {
            // NOTE: if this list of dynamic patches gets any longer look at doing this smarter...
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.whyisthat.firestarter.defsloaded");

            FirePatches.DoDefaultFirePatches(harmony);

            if (ModLister.AllInstalledMods.FirstOrDefault(m => m.Name == combatExtended_ModName)?.Active == true)
            {
                Log.Message("Firestarter: Combat Extended detected.");
                FireArrowPatches.DoCEFireArrowPatches(harmony);
            }
            else
                FireArrowPatches.DoFireArrowPatches(harmony);

        }

        public override void SceneLoaded(Scene scene)
        {
            // NOTE: the play scene appears to be the only one being loaded.
            if (scene.name == "Play" ) DynamicResearchHelper.SetGreekFireResearch(FirestarterMod.settings.enableResearch);
        }
    }
}
