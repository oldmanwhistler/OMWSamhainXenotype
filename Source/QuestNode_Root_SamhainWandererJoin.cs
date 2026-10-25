using RimWorld.QuestGen;
using Verse;

namespace SamhainXenotype;

public class QuestNode_Root_SamhainWandererJoin : QuestNode_Root_WandererJoin_WalkIn
{
    public override Pawn GeneratePawn_NewTemp(Map map)
    {
        Pawn pawn = base.GeneratePawn_NewTemp(map);
        if (pawn.Name is NameTriple generatedName)
            pawn.Name = new NameTriple(generatedName.First, "Sam", generatedName.Last);
        return pawn;
    }
}
