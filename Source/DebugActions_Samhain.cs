using System.Collections.Generic;
using LudeonTK;
using RimWorld;
using Verse;

namespace SamhainXenotype;

public static class DebugActions_Samhain
{
    [DebugAction("Pawns", "Spawn Samhain", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void SpawnSamhain()
    {
        Map map = Find.CurrentMap;
        if (map == null) return;

        var request = new PawnGenerationRequest(
            PawnKindDefOf.Villager,
            Faction.OfPlayer,
            PawnGenerationContext.PlayerStarter,
            map.Tile,
            forceGenerateNewPawn: true,
            allowDead: false,
            allowDowned: false,
            forcedXenotype: DefDatabase<XenotypeDef>.GetNamed("OMW_Samhain"));
        Pawn pawn = PawnGenerator.GeneratePawn(request);
        IntVec3 clickedCell = UI.MouseCell();
        IntVec3 cell = SpawnCellNear(map, clickedCell, useExactCell: true);
        GenSpawn.Spawn(pawn, cell, map);
    }

    [DebugAction("Pawns", "Spawn all adult costumes", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void SpawnAllAdultCostumes()
    {
        SpawnAllCostumes(child: false, UI.MouseCell());
    }

    [DebugAction("Pawns", "Spawn all child costumes", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void SpawnAllChildCostumes()
    {
        SpawnAllCostumes(child: true, UI.MouseCell());
    }

    private static void SpawnAllCostumes(bool child, IntVec3 clickedCell)
    {
        Map map = Find.CurrentMap;
        if (map == null)
            return;

        SamhainCostumePoolDef? poolDef = DefDatabase<SamhainCostumePoolDef>.GetNamedSilentFail("OMW_SamhainCostumePool");
        if (poolDef == null)
            return;

        List<SamhainCostumeSet> costumes = child ? poolDef.childCostumes : poolDef.adultCostumes;
        IntVec3 placementCenter = clickedCell.InBounds(map) ? clickedCell : map.Center;
        SamhainXenotypeSettings? settings = SamhainXenotypeMod.ActiveSettings;
        foreach (SamhainCostumeSet? costume in costumes)
        {
            if (costume?.headwear == null || (settings != null && !settings.IsCostumeEnabled(costume)))
                continue;

            SpawnCostumeApparel(costume.headwear, placementCenter, map);
            if (costume.bodyPiece != null)
                SpawnCostumeApparel(costume.bodyPiece, placementCenter, map);
        }
    }

    private static void SpawnCostumeApparel(ThingDef apparelDef, IntVec3 cell, Map map)
    {
        Apparel? apparel = SamhainCostumeGenerator.MakeCostumeApparel(apparelDef);
        if (apparel == null)
            return;

        if (!GenPlace.TryPlaceThing(apparel, cell, map, ThingPlaceMode.Near))
            apparel.Destroy();
    }

    private static IntVec3 SpawnCellNear(Map map, IntVec3 origin, bool useExactCell)
    {
        if (!origin.InBounds(map))
            origin = map.Center;

        if (useExactCell && origin.Standable(map))
            return origin;

        return CellFinder.RandomClosewalkCellNear(origin, map, 6);
    }
}
