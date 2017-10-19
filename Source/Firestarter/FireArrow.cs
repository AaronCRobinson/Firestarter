using Verse;
using UnityEngine;
using Harmony;

namespace Firestarter
{
    internal class FireArrowPatches
    {

        public static void DoFireArrowPatches(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(Projectile), nameof(Projectile.Launch), new[] { typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(Thing) }), null, new HarmonyMethod(typeof(FireArrowPatches), nameof(CheckAndSetFireArrows)));
            harmony.Patch(AccessTools.Method(typeof(Projectile), "Impact"), new HarmonyMethod(typeof(FireArrowPatches), nameof(ImpactPrefix)), null);
        }

        public static void DoCEFireArrowPatches(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(CombatExtended.ProjectileCE), nameof(CombatExtended.ProjectileCE.Launch), new[] { typeof(Thing), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(float), typeof(Thing) }), null, new HarmonyMethod(typeof(FireArrowPatches), nameof(CheckAndSetFireArrows)));
            harmony.Patch(AccessTools.Method(typeof(CombatExtended.ProjectileCE), "Impact"), new HarmonyMethod(typeof(FireArrowPatches), nameof(ImpactPrefix)), null);
        }

        public static void CheckAndSetFireArrows(Projectile __instance, Thing launcher)
        {
            __instance.GetComp<CompFireArrow>()?.SetFireArrow(launcher.Position, launcher.Map);
        }

        public static void ImpactPrefix(Projectile __instance, Thing hitThing)
        {
            if (__instance.GetComp<CompFireArrow>()?.FireArrow == true)
                FirestarterUtility.StartFire(hitThing, __instance.Position);
        }

    }

}
