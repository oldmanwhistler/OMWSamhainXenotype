using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SamhainXenotype;

[StaticConstructorOnStartup]
public static class SamhainCostumeGenerator
{
    private const string SamhainXenotypeDefName = "OMW_Samhain";
    private const string CostumePoolDefName = "OMW_SamhainCostumePool";
    private static readonly Harmony HarmonyInstance = new("oldmanwhistler.SamhainXenotype.Costumes");

    static SamhainCostumeGenerator()
    {
        MethodInfo? generatePawn = AccessTools.Method(
            typeof(PawnGenerator),
            nameof(PawnGenerator.GeneratePawn),
            new[] { typeof(PawnGenerationRequest) });
        MethodInfo? postfix = AccessTools.Method(typeof(SamhainCostumeGenerator), nameof(ApplyCostumePostfix));
        if (generatePawn == null || postfix == null)
        {
            Log.Warning("[OMW] Could not patch PawnGenerator for Samhain costume assignment.");
            return;
        }

        HarmonyInstance.Patch(generatePawn, postfix: new HarmonyMethod(postfix));
    }

    private static void ApplyCostumePostfix(Pawn __result)
    {
        if (__result?.genes == null || __result.apparel == null)
            return;

        XenotypeDef? samhain = DefDatabase<XenotypeDef>.GetNamedSilentFail(SamhainXenotypeDefName);
        if (samhain == null || __result.genes.Xenotype != samhain)
            return;

        SamhainCostumePoolDef? poolDef = DefDatabase<SamhainCostumePoolDef>.GetNamedSilentFail(CostumePoolDefName);
        if (poolDef == null)
            return;

        List<SamhainCostumeSet> pool = __result.DevelopmentalStage == DevelopmentalStage.Child
            ? poolDef.childCostumes
            : __result.DevelopmentalStage == DevelopmentalStage.Adult
                ? poolDef.adultCostumes
                : new List<SamhainCostumeSet>();

        var available = new List<SamhainCostumeSet>();
        foreach (SamhainCostumeSet? costume in pool)
        {
            if (CanWearCostume(__result, costume))
                available.Add(costume!);
        }

        if (available.TryRandomElement(out SamhainCostumeSet selected))
            TryWearCostume(__result, selected);
    }

    public static bool TryWearCostume(Pawn pawn, SamhainCostumeSet? costume, bool clearExistingOuterwearAndHeadwear = false)
    {
        if (!CanWearCostume(pawn, costume))
            return false;

        if (clearExistingOuterwearAndHeadwear)
            RemoveExistingOuterwearAndHeadwear(pawn);

        if (costume!.bodyPiece != null)
            Wear(pawn, costume.bodyPiece);
        Wear(pawn, costume.headwear);
        return true;
    }

    private static bool CanWearCostume(Pawn pawn, SamhainCostumeSet? costume)
    {
        if (costume?.headwear == null
            || (SamhainXenotypeMod.ActiveSettings != null && !SamhainXenotypeMod.ActiveSettings.IsCostumeEnabled(costume))
            || !CanWear(pawn, costume.headwear))
            return false;

        if (costume.bodyPiece == null)
            return true;

        return CanWear(pawn, costume.bodyPiece)
            && ApparelUtility.CanWearTogether(costume.bodyPiece, costume.headwear, pawn.RaceProps.body);
    }

    private static bool CanWear(Pawn pawn, ThingDef? def)
    {
        if (def?.IsApparel != true
            || !def.apparel.PawnCanWear(pawn, ignoreGender: true)
            || !ApparelUtility.HasPartsToWear(pawn, def)
            || def.apparel.layers.NullOrEmpty())
            return false;

        foreach (ApparelLayerDef layer in def.apparel.layers)
        {
            if (layer.defName != "Shell" && layer.defName != "Overhead")
                return false;
        }
        return true;
    }

    private static void RemoveExistingOuterwearAndHeadwear(Pawn pawn)
    {
        List<Apparel> worn = pawn.apparel.WornApparel;
        for (int i = worn.Count - 1; i >= 0; i--)
        {
            Apparel apparel = worn[i];
            if (!HasOnlyOuterwearOrHeadwearLayers(apparel.def))
                continue;

            pawn.apparel.Remove(apparel);
            apparel.Destroy();
        }
    }

    private static bool HasOnlyOuterwearOrHeadwearLayers(ThingDef def)
    {
        if (def.apparel?.layers.NullOrEmpty() != false)
            return false;

        foreach (ApparelLayerDef layer in def.apparel.layers)
        {
            if (layer.defName != "Shell" && layer.defName != "Overhead")
                return false;
        }
        return true;
    }

    public static Apparel? MakeCostumeApparel(ThingDef? def)
    {
        if (def?.IsApparel != true)
            return null;

        ThingDef? stuff = def.MadeFromStuff ? GenStuff.DefaultStuffFor(def) : null;
        return ThingMaker.MakeThing(def, stuff) as Apparel;
    }

    private static void Wear(Pawn pawn, ThingDef? def)
    {
        Apparel? apparel = MakeCostumeApparel(def);
        if (apparel != null && apparel.PawnCanWear(pawn, ignoreGender: true))
            pawn.apparel.Wear(apparel, dropReplacedApparel: true);
        else
            apparel?.Destroy();
    }

}
