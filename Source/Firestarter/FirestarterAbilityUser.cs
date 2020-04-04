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
        public bool? firestarter;

        // Provides ability without affecting save.a
        public override void CompTick()
        {
            if (AbilityUser?.Spawned == true)
            {
                if (firestarter != null)
                {
                    if (firestarter == true)
                        base.CompTick();
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
            if (firestarter == true) this.AddPawnAbility(AbilityDefOf.Firestarter);
        }

        public bool IsFirestarter
        {
            get
            {
                bool val = false;
                if (this.AbilityUser != null)
                {
                    val = !this.AbilityUser.story.DisabledWorkTagsBackstoryAndTraits.HasFlag(WorkTags.Violent);
                    if (FirestarterMod.settings.onlyPyro) val &= this.AbilityUser.story.traits.HasTrait(TraitDefOf.Pyromaniac);
                }
                return val;
            }
        }

        public override bool TryTransformPawn()
        {
            if (!FirestarterMod.settings.enableFirestarterAbility) return false;
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
                for (int i = 0; i < this.AbilityData.AllPowers.Count; i++)
                    yield return this.AbilityData.AllPowers[i].GetGizmo();
            }
        }

    }

    [DefOf]
    public static class AbilityDefOf
    {
        public static AbilityUser.AbilityDef Firestarter;
    }

    public class FirestarterSpark : Projectile_AbilityBase
    {
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            FirestarterUtility.StartFire(hitThing, DestinationCell);
        }
    }

}   