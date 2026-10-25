using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SamhainXenotype;

public class PsychicRitualDef_SamhainRecruit : PsychicRitualDef_InvocationCircle
{
    public override PsychicRitualCandidatePool FindCandidatePool()
    {
        var candidates = new List<Pawn>(Find.CurrentMap.mapPawns.FreeColonistsAndPrisonersSpawned);
        candidates.AddRange(Find.CurrentMap.mapPawns.SlavesOfColonySpawned);
        return new PsychicRitualCandidatePool(candidates, new List<Pawn>());
    }

    public override List<PsychicRitualToil> CreateToils(PsychicRitual psychicRitual, PsychicRitualGraph parent)
    {
        List<PsychicRitualToil> toils = base.CreateToils(psychicRitual, parent);
        toils.Add(new PsychicRitualToil_SamhainRecruit(InvokerRole, TargetRole));
        toils.Add(new PsychicRitualToil_TargetCleanup(InvokerRole, TargetRole));
        return toils;
    }
}

public class PsychicRitualToil_SamhainRecruit : PsychicRitualToil
{
    private PsychicRitualRoleDef invokerRole = null!;
    private PsychicRitualRoleDef targetRole = null!;
    private bool outcomeApplied;

    protected PsychicRitualToil_SamhainRecruit()
    {
    }

    public PsychicRitualToil_SamhainRecruit(PsychicRitualRoleDef invokerRole, PsychicRitualRoleDef targetRole)
    {
        this.invokerRole = invokerRole;
        this.targetRole = targetRole;
    }

    public override void Start(PsychicRitual psychicRitual, PsychicRitualGraph parent)
    {
        base.Start(psychicRitual, parent);
        if (outcomeApplied)
            return;
        outcomeApplied = true;

        Pawn invoker = psychicRitual.assignments.FirstAssignedPawn(invokerRole);
        Pawn sacrifice = psychicRitual.assignments.FirstAssignedPawn(targetRole);
        if (sacrifice != null && !sacrifice.Dead)
        {
            sacrifice.Kill(null);
            if (invoker != null)
                PsychicRitualUtility.RegisterAsExecutionIfPrisoner(sacrifice, invoker);
        }

        Map map = psychicRitual.Map;
        var request = new PawnGenerationRequest(
            PawnKindDefOf.Villager,
            null,
            PawnGenerationContext.NonPlayer,
            map.Tile,
            forceGenerateNewPawn: true,
            allowDead: false,
            allowDowned: false,
            forcedXenotype: DefDatabase<XenotypeDef>.GetNamed("OMW_Samhain"));
        var slate = new RimWorld.QuestGen.Slate();
        slate.Set("map", map);
        slate.Set("overridePawnGenParams", request);
        QuestUtility.GenerateQuestAndMakeAvailable(DefDatabase<QuestScriptDef>.GetNamed("OMW_SamhainWandererJoins"), slate);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look(ref invokerRole, "invokerRole");
        Scribe_Defs.Look(ref targetRole, "targetRole");
        Scribe_Values.Look(ref outcomeApplied, "outcomeApplied", false);
    }
}
