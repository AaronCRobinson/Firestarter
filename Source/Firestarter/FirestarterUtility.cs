using RimWorld;
using Verse;

namespace Firestarter
{
    [StaticConstructorOnStartup]
    public static class FirestarterUtility
    {
        private const float defaultFireSize = 0.5f;
        private static Graphic graphicInt;

        public static void StartFire(Thing hitThing, IntVec3 destinationCell)
        {
            Fire fire = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire, null);
            fire.fireSize = defaultFireSize;
            if (hitThing != null)
            {
                if (hitThing is Pawn) hitThing.TryAttachFire(defaultFireSize);
                else GenSpawn.Spawn(fire, hitThing.Position, Find.VisibleMap, Rot4.North, false);
            }
            else
            {
                GenSpawn.Spawn(fire, destinationCell, Find.VisibleMap, Rot4.North, false);
            }
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

        public static Graphic XenarrowGraphic
        {
            get
            {
                if (graphicInt == null)
                {
                    GraphicData graphicData = new GraphicData()
                    {
                        texPath = "Projectile/Xenarrow",
                        graphicClass = typeof(Graphic_Single),
                        shaderType = ShaderType.TransparentPostLight
                    };
                    graphicInt = graphicData.Graphic;
                }
                return graphicInt;
            }
        }
    }

}
