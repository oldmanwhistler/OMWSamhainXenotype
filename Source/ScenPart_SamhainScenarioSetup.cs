using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SamhainXenotype;

public class ScenPart_SamhainScenarioSetup : ScenPart
{
    public override void PreConfigure()
    {
        base.PreConfigure();
        if (Find.GameInitData != null)
            Find.GameInitData.startingSeason = Season.Fall;
    }

    public override void PostMapGenerate(Map map)
    {
        base.PostMapGenerate(map);
        Faction hostileAncients = Find.FactionManager.FirstFactionOfDef(FactionDefOf.AncientsHostile);
        if (hostileAncients == null)
        {
            FactionGenerator.CreateFactionAndAddToManager(FactionDefOf.AncientsHostile);
            hostileAncients = Find.FactionManager.FirstFactionOfDef(FactionDefOf.AncientsHostile);
        }
        if (hostileAncients == null)
        {
            Log.Error("[OMW] Samhain Xenotype: failed to create the AncientsHostile faction; scenario enemy was not spawned.");
            return;
        }

        IntVec3 start = MapGenerator.PlayerStartSpot;
        IntVec3 spawnCell = IntVec3.Invalid;
        int greatestDistance = -1;
        foreach (IntVec3 cell in map.AllCells)
        {
            if (cell.x > 1 && cell.x < map.Size.x - 2 && cell.z > 1 && cell.z < map.Size.z - 2)
                continue;
            if (!cell.Standable(map))
                continue;
            int distance = cell.DistanceToSquared(start);
            if (distance > greatestDistance)
            {
                greatestDistance = distance;
                spawnCell = cell;
            }
        }

        if (!spawnCell.IsValid)
        {
            Log.Error("[OMW] Samhain Xenotype: could not find a standable map-edge cell for the hostile Ancient.");
            return;
        }

        var request = new PawnGenerationRequest(
            PawnKindDefOf.AncientSoldier,
            hostileAncients,
            PawnGenerationContext.NonPlayer,
            map.Tile,
            forceGenerateNewPawn: true,
            allowDead: false,
            allowDowned: false,
            forcedXenotype: XenotypeDefOf.Baseliner);
        Pawn enemy = PawnGenerator.GeneratePawn(request);
        GenSpawn.Spawn(enemy, spawnCell, map);
        LordMaker.MakeNewLord(
            hostileAncients,
            new LordJob_AssaultColony(hostileAncients, canKidnap: false, canTimeoutOrFlee: false, canSteal: false),
            map,
            new List<Pawn> { enemy });
    }
}
