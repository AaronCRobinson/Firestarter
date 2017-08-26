using CombatExtended;
using Harmony;
using HugsLib;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;
using Verse;

namespace Firestarter
{
    public class FirestarterModBase : ModBase
    {
        public override string ModIdentifier
        {
            get { return "Firestarter"; }
        }

        public override void DefsLoaded()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.whyisthat.firestarter.defsloaded");

            // trying NoFirewatcher (if/else)
            try { TryDoCustomFirePatches(harmony); }
            catch (TypeLoadException) { FirePatches.DoDefaultFirePatches(harmony); }

            // trying Combat Extended (if/else)
            try { TryDoCustomFireArrowPatches(harmony); }
            catch (TypeLoadException) { FireArrowPatches.DoFireArrowPatches(harmony); }
        }

        #region DynamicPatches


        // thanks erdelf
        private void TryDoCustomFirePatches(HarmonyInstance harmony)
        {
            ((Action)(() =>
            {
                // NOTE: maybe use a getter here?
                if (AccessTools.Method(typeof(NoFirewatcher.HighPerformanceFire), nameof(NoFirewatcher.HighPerformanceFire.Tick)) != null)
                {
                    Log.Message("Firestarter: NoFirewatcher detected.");
                    FirePatches.DoCustomFirePatches(harmony);
                }
            }))();
        }

        private void TryDoCustomFireArrowPatches(HarmonyInstance harmony)
        {
            ((Action)(() =>
            {

                MethodInfo MI_launchProjectileCE = AccessTools.Method(typeof(Verb_LaunchProjectileCE), "TryCastShot");

                if (MI_launchProjectileCE != null)
                {
                    Log.Message("Firestarter: Combat Extended detected.");
                    FireArrowPatches.DoCEFireArrowPatches(harmony);
                }
            }))();
        }

        #endregion

        public override void SceneLoaded(Scene scene)
        {
            // NOTE: the play scene appears to be the only one being loaded.
            if (scene.name == "Play" ) DynamicResearchHelper.SetGreekFireResearch(FirestarterMod.settings.enableResearch);
        }
    }
}
