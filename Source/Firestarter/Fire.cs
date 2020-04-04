using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Reflection.Emit;

namespace Firestarter
{
    [StaticConstructorOnStartup]
    internal class FirePatches
    {
        static FirePatches()
        {
            Harmony harmony = new Harmony("rimworld.whyisthat.firestarter.fire");

            // TODO: fix this patch... missing label after dropping a block of code.
            //harmony.Patch(AccessTools.Method(typeof(FireUtility), nameof(FireUtility.ChanceToStartFireIn)), null, null, new HarmonyMethod(typeof(FirePatches), nameof(ChanceToStartFireIn_UseFlammabilityMax)));
        }

        public static void DoDefaultFirePatches(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(Fire), "DoComplexCalcs"), null, null, new HarmonyMethod(typeof(FirePatches), nameof(FireSizeTranspiler)));
            harmony.Patch(AccessTools.Property(typeof(Fire), "SpreadInterval").GetGetMethod(true), null, null, new HarmonyMethod(typeof(FirePatches), nameof(FixFireSpreadIntervalTranspiler)));
        }

        // NOTE: Look into the impact of this change...
        public static IEnumerable<CodeInstruction> FixFireSpreadIntervalTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4)
                {
                    if ((float)instruction.operand == 150f) instruction.operand = 600f;
                    if ((float)instruction.operand == 40f) instruction.operand = 160f;
                }
                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> FireSizeTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            CodeInstruction instruction;
            for (int i = 0; i < instructionList.Count; i++)
            {
                instruction = instructionList[i];
                if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 0.00055f)
                {
                    // throw away current instruction
                    while (instructionList[++i].opcode != OpCodes.Mul) { yield return instructionList[i]; } // get flammabilityMax on the stack
                    while (instructionList[++i].opcode != OpCodes.Add) { } // eating instructions
                    yield return new CodeInstruction(OpCodes.Call, typeof(StartFireHelper).GetMethod(nameof(StartFireHelper.FireGrowthAmount)));
                    yield return instructionList[i];
                    yield return instructionList[++i];
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static IEnumerable<CodeInstruction> ChanceToStartFireIn_UseFlammabilityMax(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (instructionList[i].opcode == OpCodes.Stloc_1)
                {
                    while (instructionList[++i].opcode != OpCodes.Ldloc_1) { } // remove block
                    yield return new CodeInstruction(OpCodes.Ldarg_0); // c
                    yield return new CodeInstruction(OpCodes.Ldarg_1); // map
                    yield return new CodeInstruction(OpCodes.Ldloc_1); // flammabilityMax
                    yield return new CodeInstruction(OpCodes.Call, typeof(StartFireHelper).GetMethod(nameof(StartFireHelper.ChanceUsingFlammabilityMax)));
                    yield return new CodeInstruction(OpCodes.Stloc_1);
                    for (; i < instructionList.Count; i++) yield return instructionList[i]; // yield leftovers
                }
                else
                {
                    yield return instructionList[i];
                }
            }
        }
    }

    static class StartFireHelper
    {
        static float compareValue; // UseFlammabilityMaxChance => temp var

        public static float FireGrowthAmount(float flammabilityMax)
        {
            if (flammabilityMax <= 0.5f) return (Rand.Value * 0.5f - 0.5f) * 0.165f; // low flammability => chance
            return (flammabilityMax - 0.5f) * 0.165f; // high flammability => no chance
        }

        public static float ChanceUsingFlammabilityMax(IntVec3 c, Map map, ref float flammabilityMax)
        {
            List<Thing> thingList = c.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i] is Fire)
                {
                    flammabilityMax = 0f;
                    return flammabilityMax;
                }
                if (thingList[i].FlammableNow) // NOTE: this should be patched..
                {
                    compareValue = thingList[i].GetStatValue(StatDefOf.Flammability, true);
                    if (compareValue > flammabilityMax) flammabilityMax = compareValue;
                }
            } // flammabilityMax found
            // return chance
            return Mathf.Pow(5 - flammabilityMax, -2); // = 1/x^2
        }
    }

}