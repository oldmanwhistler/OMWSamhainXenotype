using System.Collections.Generic;
using Verse;

namespace SamhainXenotype;

public class SamhainCostumePoolDef : Def
{
    public List<SamhainCostumeSet> adultCostumes = new();
    public List<SamhainCostumeSet> childCostumes = new();

    public override void ResolveReferences()
    {
        base.ResolveReferences();
        adultCostumes.RemoveAll(costume => !CoversFullHead(costume));
        childCostumes.RemoveAll(costume => !CoversFullHead(costume));
    }

    private static bool CoversFullHead(SamhainCostumeSet? costume)
    {
        return CoversFullHead(costume?.headwear) || CoversFullHead(costume?.bodyPiece);
    }

    private static bool CoversFullHead(ThingDef? apparel)
    {
        if (apparel?.apparel?.bodyPartGroups == null)
            return false;

        foreach (BodyPartGroupDef group in apparel.apparel.bodyPartGroups)
        {
            if (group.defName == "FullHead")
                return true;
        }
        return false;
    }
}

public class SamhainCostumeSet
{
    public ThingDef? headwear;
    public ThingDef? bodyPiece;
    public bool defaultEnabled = true;
}
