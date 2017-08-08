using System;
using System.Collections.Generic;
using Verse;
using Harmony;

namespace Firestarter
{
    // NOTE: consider building some kind of def database here too?

    public static class DynamicResearchHelper
    {
        private static Dictionary<string, ResearchProjectDef> defs = new Dictionary<string, ResearchProjectDef>();

        // TEMP
        private const string researchName = "GreekFireProduction";
        private const string thingName = "GreekFire";

        public static void SetGreekFireResearch(bool add)
        {
            if (add)
            {
                if (DefDatabase<ResearchProjectDef>.GetNamed(researchName, false) == null)
                {   // Add def and references
                    defs.TryGetValue(researchName, out ResearchProjectDef research);
                    DefDatabase<ResearchProjectDef>.Add(research);
                    BuildableDef building = DefDatabase<ThingDef>.GetNamed(thingName) as BuildableDef;
                    building.researchPrerequisites = new List<ResearchProjectDef>() { research };
                    //Find.WindowStack.WindowOfType<MainTabWindow_Research>()?.WindowOnGUI();
                }
            }
            else
            {
                if (DefDatabase<ResearchProjectDef>.GetNamed(researchName, false) != null)
                {   // Remove def and references
                    ResearchProjectDef research = DefDatabase<ResearchProjectDef>.GetNamed(researchName);
                    defs.Add(researchName, research);
                    AccessTools.Method(typeof(DefDatabase<ResearchProjectDef>), "Remove").Invoke(null, new[] { research });
                    BuildableDef building = DefDatabase<ThingDef>.GetNamed(thingName) as BuildableDef;
                    building.researchPrerequisites = null;
                    //Find.WindowStack.WindowOfType<MainTabWindow_Research>()?.WindowOnGUI();
                }
            }
        }
    }
}
