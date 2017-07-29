using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;
using Harmony;
using System.Reflection.Emit;
using System.Reflection;

namespace Firestarter
{
    [StaticConstructorOnStartup]
    class FireArrowPatches
    {
        static FireArrowPatches()
        {
            //HarmonyInstance.DEBUG = true;
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.whyisthat.firestarter.firearrow");

            harmony.Patch(AccessTools.Method(typeof(Verb_LaunchProjectile), "TryCastShot"), null, null, new HarmonyMethod(typeof(FireArrowPatches), nameof(TryCastShotTranspiler)));
        }

        private static MethodInfo spawnMethodInfo = AccessTools.Method(typeof(GenSpawn), nameof(GenSpawn.Spawn), new Type[] { typeof(ThingDef), typeof(IntVec3), typeof(Map) });
        public static IEnumerable<CodeInstruction> TryCastShotTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            int i;
            for (i = 0; i < instructionList.Count; i++)
            {
                yield return instructionList[i];
                if (instructionList[i].opcode == OpCodes.Stloc_2)
                {
                    while (instructionList[++i].opcode != OpCodes.Call || instructionList[i].operand != spawnMethodInfo)
                    {
                        yield return instructionList[i];
                    }

                    yield return new CodeInstruction(OpCodes.Call, typeof(FireArrowHelper).GetMethod(nameof(FireArrowHelper.CreateProjectile)));

                    while (instructionList[++i].opcode != OpCodes.Stloc_3) { }
                    break;
                }
            }
            // finish off the instruction list
            for (; i < instructionList.Count; i++) { yield return instructionList[i]; }
        }
    }

    static class FireArrowHelper
    {
        public static Projectile CreateProjectile(ThingDef projectileDef, IntVec3 source, Map map)
        {
            Thing projectile;

            if (projectileDef.projectile.damageDef == DamageDefOf.Arrow)
            {
                bool fireArrow = source.GetThingList(map).Any(t => t.def == ThingDefOf.TorchLamp || t.def == ThingDefOf.Campfire);
                if (!fireArrow)
                {
                    // check adjacent
                    for (int i = 0; i < 8; i++)
                    {
                        IntVec3 c2 = source + GenAdj.AdjacentCells[i];
                        fireArrow = c2.GetThingList(map).Any(t => t.def == ThingDefOf.TorchLamp || t.def == ThingDefOf.Campfire);
                        if (fireArrow) break;
                    }
                }

                if (!fireArrow)
                    projectile = (Thing)Activator.CreateInstance(projectileDef.thingClass);
                else
                    projectile = (Thing)Activator.CreateInstance(typeof(Projectile_FireArrow));
            }
            else // not an arrow
            { 
                projectile = (Thing)Activator.CreateInstance(projectileDef.thingClass);
            }

            projectile.def = projectileDef;
            projectile.PostMake();
            return (Projectile)GenSpawn.Spawn(projectile, source, map);
        }
    }

    // NOTE: almost the same as FirestarterSpark => look into asbtraction or something
    [StaticConstructorOnStartup]
    public class Projectile_FireArrow : Projectile
    {
        private const float defaultFireSize = 0.5f;
        private static Graphic graphicInt;

        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            Fire fire = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire, null);
            fire.fireSize = defaultFireSize;
            if (hitThing != null)
            {
                if (hitThing is Pawn) hitThing.TryAttachFire(defaultFireSize);
                else GenSpawn.Spawn(fire, hitThing.Position, Find.VisibleMap, Rot4.North, false);
            }
            else
            {
                GenSpawn.Spawn(fire, DestinationCell, Find.VisibleMap, Rot4.North, false);
            }
        }

        public new Graphic Graphic
        {
            get
            {
                if (graphicInt == null)
                {
                    Log.Message("graphicInt");
                    GraphicData graphicData = new GraphicData();
                    graphicData.texPath = "Projectile/Xenarrow";
                    graphicData.graphicClass = typeof(Graphic_Single);
                    graphicData.shaderType = ShaderType.TransparentPostLight;
                    graphicInt = graphicData.Graphic;
                }
                return graphicInt;
            }
        }

        public override void Draw()
        {
            Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.Graphic.MatSingle, 0);
            base.Comps_PostDraw();
        }

    }

}
