using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AbilityUser;
using UnityEngine;

namespace Firestarter
{
    [StaticConstructorOnStartup]
    public static class TexButton
    {
        public static readonly Texture2D FireAbility = ContentFinder<Texture2D>.Get("UI/Fire", true);
    }

    // reference: https://github.com/roxxploxx/RimWorldModGuide/wiki/SHORTUTORIAL%3A-JecsTools.CompAbilityUser

    public class CompFirestarter : GenericCompAbilityUser //GenericCompAbilityUser
    {
        public static FirestarterSettings settings;
        public bool? firestarter;

        // Provides ability without affecting save.a
        public override void CompTick()
        {
            if (AbilityUser?.Spawned == true)
            {
                if (firestarter != null)
                {
                    if (firestarter == true)
                    {
                        base.CompTick();
                    }
                }
                else
                {
                    firestarter = TryTransformPawn();
                    Initialize();
                }
            }

        }

        public override void PostInitialize()
        {
            base.PostInitialize();
            if (firestarter == true) this.AddPawnAbility(FirestarterDefOf.Firestarter);
        }

        public bool IsFirestarter
        {
            get
            {
                bool val = false;
                if (this.AbilityUser != null)
                {
                    val = !this.AbilityUser.story.WorkTagIsDisabled(WorkTags.Violent);
                    if (settings.onlyPyro) val &= this.AbilityUser.story.traits.HasTrait(TraitDefOf.Pyromaniac);
                }
                return val;
            }
        }

        public override bool TryTransformPawn()
        {
            if (!settings.enableFirestarterAbility) return false;
            return IsFirestarter;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (this.AbilityUser.Drafted)
            {
                IEnumerator<Gizmo> enumerator = base.CompGetGizmosExtra().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Gizmo current = enumerator.Current;
                    yield return current;
                }
                foreach (Command_Target comm in GetPawnAbilityVerbs().ToList())
                {
                    yield return comm;
                }
            }
        }

    }

    [DefOf]
    public static class FirestarterDefOf
    {
        public static AbilityDef Firestarter;
    }

    public class FirestarterSpark : Projectile_AbilityBase
    {
        private const float defaultFireSize = 0.5f;

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
    }

}   