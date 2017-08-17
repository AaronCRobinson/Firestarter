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
        public static FirestarterSettings settings;

        public FirestarterMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<FirestarterSettings>();
        }

        public override string SettingsCategory() => "FirestarterSettingsCategoryLabel".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            ModWindowHelper.Reset();
            ModWindowHelper.MakeLabeledCheckbox(inRect, "EnableFirestarterAbilityLabel".Translate() + ": ", ref settings.enableFirestarterAbility);
            ModWindowHelper.MakeLabeledCheckbox(inRect, "OnlyPyroLabel".Translate() + ": ", ref settings.onlyPyro);
            ModWindowHelper.MakeLabeledCheckbox(inRect, "EnableResearch".Translate() + ": ", ref settings.enableResearch);
            settings.Write();
        }
    }
}
