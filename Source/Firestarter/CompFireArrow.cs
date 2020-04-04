using UnityEngine;
using RimWorld;
using Verse;
using HarmonyLib;

namespace Firestarter
{
    public class CompFireArrow : ThingComp
    {
        bool fireArrow = false;
        Traverse traverseParent;

        Traverse TraverseParent
        {
            get
            {
                if (traverseParent == null) traverseParent = Traverse.Create(this.parent);
                return traverseParent;
            }
        }

        public bool FireArrow { get => fireArrow; }

        public bool SetFireArrow(IntVec3 source, Map map)
        {
            return fireArrow = CheckAdjacentFireDevice(source, map);
        }

        public static bool CheckAdjacentFireDevice(IntVec3 source, Map map)
        {
            bool fireArrow = source.GetThingList(map).Any(t => t.def == ThingDefOf.TorchLamp || t.def == ThingDefOf.Campfire);
            if (!fireArrow)
            {
                for (int i = 0; i < 8; i++)
                {
                    IntVec3 c2 = source + GenAdj.AdjacentCells[i];
                    if (!c2.InBounds(map)) continue;
                    fireArrow = c2.GetThingList(map).Any(t => t.def == ThingDefOf.TorchLamp || t.def == ThingDefOf.Campfire);
                    if (fireArrow) break;
                }
            }
            return fireArrow;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref fireArrow, "fireArrow", false);
        }

        public override void PostDraw()
        {
            if (this.fireArrow)
                Graphics.DrawMesh(MeshPool.plane10, this.parent.DrawPos, TraverseParent.Property("ExactRotation").GetValue<Quaternion>(), FirestarterUtility.XenarrowGraphic.MatSingle, 0);
        }

    }

    public class CompProperties_FireArrow : CompProperties
    {
        public CompProperties_FireArrow()
        { 
            compClass = typeof(CompFireArrow);
        }
    }

}
