using Verse;

namespace Firestarter
{
    class Firestarter_GameComponent : GameComponent
    {
        public Firestarter_GameComponent(Game g) { }

        public Firestarter_GameComponent() { }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            DynamicResearchHelper.SetGreekFireResearch(FirestarterMod.settings.enableResearch);
        }
    }
}
