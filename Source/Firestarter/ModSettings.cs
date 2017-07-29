using Verse;
using UnityEngine;

namespace Firestarter
{
    public class FirestarterSettings : ModSettings
    {
        public bool enableFirestarterAbility = true;
        public bool onlyPyro = false;
        public bool enableResearch = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.enableFirestarterAbility, "enableFirestarterAbility", true);
            Scribe_Values.Look(ref this.onlyPyro, "onlyPyro", false);
            Scribe_Values.Look(ref this.enableResearch, "enableResearch", true);
        }
    }

    class FirestarterMod : Mod
    {
        FirestarterSettings settings;

        public FirestarterMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<FirestarterSettings>();
            CompFirestarter.settings = this.settings;
        }

        public override string SettingsCategory() => "Firestarter";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            // reference: https://github.com/erdelf/GodsOfRimworld/blob/master/Source/Ankh/ModControl.cs
            // reference: https://github.com/erdelf/PrisonerRansom/
            float num = 30f;

            Widgets.Label(new Rect(0f, num + 5, inRect.width - 16f, 40f), "Enable Firestarter ability: ");
            Widgets.Checkbox(inRect.width - 64f, num + 6f, ref this.settings.enableFirestarterAbility);

            num += 25;

            Widgets.Label(new Rect(0f, num + 5, inRect.width - 16f, 40f), "Only pyromaniac has Firestarter ability: ");
            Widgets.Checkbox(inRect.width - 64f, num + 6f, ref this.settings.onlyPyro);

            num += 25;

            Widgets.Label(new Rect(0f, num + 5, inRect.width - 16f, 40f), "Enable research requirement for Greek Fire: ");
            Widgets.Checkbox(inRect.width - 64f, num + 6f, ref this.settings.enableResearch);

            this.settings.Write();
        }
    }
}
