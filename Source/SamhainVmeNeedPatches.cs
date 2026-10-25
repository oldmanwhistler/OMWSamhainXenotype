using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SamhainXenotype;

[StaticConstructorOnStartup]
public static class SamhainVmeNeedPatches
{
    private static readonly Harmony HarmonyInstance = new("oldmanwhistler.SamhainXenotype.VmeNeeds");

    static SamhainVmeNeedPatches()
    {
        PatchNeedVisibility("VanillaMemesExpanded.Need_Desserts", "VME_Desserts");
        PatchNeedVisibility("VanillaMemesExpanded.Need_Anonymity", "VME_Anonymity");
    }

    private static void PatchNeedVisibility(string needClassName, string needDefName)
    {
        if (DefDatabase<NeedDef>.GetNamedSilentFail(needDefName) == null)
            return;

        Type? needType = AccessTools.TypeByName(needClassName);
        MethodInfo? getter = needType == null ? null : AccessTools.PropertyGetter(needType, nameof(Need.ShowOnNeedList));
        MethodInfo? postfix = AccessTools.Method(typeof(SamhainVmeNeedPatches), nameof(ShowOnNeedListPostfix));
        if (getter == null || postfix == null)
        {
            Log.Warning($"[OMW] Could not patch VME need visibility for {needDefName}; the optional need class or property was not found.");
            return;
        }

        HarmonyInstance.Patch(getter, postfix: new HarmonyMethod(postfix));
    }

    private static void ShowOnNeedListPostfix(Need __instance, Pawn ___pawn, ref bool __result)
    {
        if (___pawn?.genes != null && ___pawn.genes.EnablesNeed(__instance.def))
            __result = true;
    }
}
