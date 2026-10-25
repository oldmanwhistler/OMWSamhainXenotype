using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SamhainXenotype;

public class SamhainXenotypeSettings : ModSettings
{
    private List<string> disabledCostumeKeys = new();
    private List<string> enabledCostumeKeys = new();

    public bool IsCostumeEnabled(SamhainCostumeSet? costume)
    {
        if (costume?.headwear == null)
            return false;

        string key = CostumeKey(costume);
        if (enabledCostumeKeys.Contains(key))
            return true;
        if (disabledCostumeKeys.Contains(key))
            return false;
        return costume.defaultEnabled;
    }

    public void SetCostumeEnabled(SamhainCostumeSet costume, bool enabled)
    {
        string key = CostumeKey(costume);
        bool isCurrentlyEnabled = IsCostumeEnabled(costume);
        if (enabled == isCurrentlyEnabled)
            return;

        disabledCostumeKeys.Remove(key);
        enabledCostumeKeys.Remove(key);
        if (enabled != costume.defaultEnabled)
        {
            if (enabled)
                enabledCostumeKeys.Add(key);
            else
                disabledCostumeKeys.Add(key);
        }
    }

    public static string CostumeKey(SamhainCostumeSet costume)
    {
        return (costume.headwear?.defName ?? string.Empty) + "|" + (costume.bodyPiece?.defName ?? string.Empty);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref disabledCostumeKeys, "disabledCostumeKeys", LookMode.Value);
        Scribe_Collections.Look(ref enabledCostumeKeys, "enabledCostumeKeys", LookMode.Value);
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            disabledCostumeKeys ??= new List<string>();
            enabledCostumeKeys ??= new List<string>();
        }
    }
}

internal static class SamhainCostumeSettingsPage
{
    private const float RowHeight = 88f;
    private const float SectionHeaderHeight = 32f;
    private const float IconSize = 52f;
    private const float CheckboxSize = 24f;

    private sealed class CostumeRow
    {
        public SamhainCostumeSet costume = null!;
        public bool forAdults;
        public bool forChildren;
    }

    private sealed class CostumeSection
    {
        public string sourceMod = string.Empty;
        public List<CostumeRow> rows = new();
    }

    public static void Draw(Rect inRect, SamhainXenotypeSettings settings, ref Vector2 scrollPosition, System.Action writeSettings)
    {
        GameFont previousFont = Text.Font;
        TextAnchor previousAnchor = Text.Anchor;
        try
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;

            SamhainCostumePoolDef? pool = DefDatabase<SamhainCostumePoolDef>.GetNamedSilentFail("OMW_SamhainCostumePool");
            if (pool == null)
            {
                Widgets.Label(inRect, "OMW_CostumePoolUnavailable".Translate());
                return;
            }

            List<CostumeSection> sections = GatherSections(pool);
            if (sections.Count == 0)
            {
                Widgets.Label(inRect, "OMW_NoSupportedCostumes".Translate());
                return;
            }

            float contentHeight = 8f;
            foreach (CostumeSection section in sections)
                contentHeight += SectionHeaderHeight + section.rows.Count * RowHeight;
            float viewHeight = Mathf.Max(inRect.height, contentHeight);
            Rect viewRect = new(0f, 0f, inRect.width - 16f, viewHeight);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);
            try
            {
                float y = 4f;
                foreach (CostumeSection section in sections)
                {
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleLeft;
                    Rect headerRect = new(4f, y, viewRect.width - 8f, SectionHeaderHeight - 2f);
                    Widgets.Label(headerRect, "OMW_CostumeSourceHeader".Translate(section.sourceMod));
                    Widgets.DrawLine(new Vector2(4f, y + SectionHeaderHeight - 2f), new Vector2(viewRect.width - 4f, y + SectionHeaderHeight - 2f), new Color(0.55f, 0.55f, 0.55f), 1f);
                    y += SectionHeaderHeight;

                    foreach (CostumeRow row in section.rows)
                    {
                        DrawRow(new Rect(0f, y, viewRect.width, RowHeight), row, settings, writeSettings);
                        y += RowHeight;
                    }
                }
            }
            finally
            {
                Widgets.EndScrollView();
            }
        }
        finally
        {
            Text.Font = previousFont;
            Text.Anchor = previousAnchor;
        }
    }

    private static List<CostumeSection> GatherSections(SamhainCostumePoolDef pool)
    {
        List<CostumeSection> sections = new();
        foreach (CostumeRow row in GatherRows(pool))
        {
            string sourceMod = ModName(row.costume.headwear!);
            CostumeSection? section = null;
            foreach (CostumeSection candidate in sections)
            {
                if (candidate.sourceMod == sourceMod)
                {
                    section = candidate;
                    break;
                }
            }

            if (section == null)
            {
                section = new CostumeSection { sourceMod = sourceMod };
                sections.Add(section);
            }
            section.rows.Add(row);
        }
        return sections;
    }

    private static List<CostumeRow> GatherRows(SamhainCostumePoolDef pool)
    {
        List<CostumeRow> rows = new();
        AddRows(rows, pool.adultCostumes, forAdults: true);
        AddRows(rows, pool.childCostumes, forAdults: false);
        return rows;
    }

    private static void AddRows(List<CostumeRow> rows, List<SamhainCostumeSet> costumes, bool forAdults)
    {
        foreach (SamhainCostumeSet? costume in costumes)
        {
            if (costume?.headwear == null)
                continue;

            string key = SamhainXenotypeSettings.CostumeKey(costume);
            CostumeRow? existing = null;
            foreach (CostumeRow row in rows)
            {
                if (SamhainXenotypeSettings.CostumeKey(row.costume) == key)
                {
                    existing = row;
                    break;
                }
            }

            if (existing != null)
            {
                if (forAdults)
                    existing.forAdults = true;
                else
                    existing.forChildren = true;
            }
            else
            {
                rows.Add(new CostumeRow
                {
                    costume = costume,
                    forAdults = forAdults,
                    forChildren = !forAdults
                });
            }
        }
    }

    private static void DrawRow(Rect rect, CostumeRow row, SamhainXenotypeSettings settings, System.Action writeSettings)
    {
        SamhainCostumeSet costume = row.costume;
        ThingDef headwear = costume.headwear!;
        bool enabled = settings.IsCostumeEnabled(costume);
        bool previousValue = enabled;
        Widgets.Checkbox(new Vector2(rect.x + 2f, rect.y + 14f), ref enabled, CheckboxSize);
        if (enabled != previousValue)
        {
            settings.SetCostumeEnabled(costume, enabled);
            writeSettings();
        }

        float iconX = rect.x + 34f;
        Rect headwearIcon = new(iconX, rect.y + 8f, IconSize, IconSize);
        Widgets.DefIcon(headwearIcon, headwear);
        AddItemTooltip(headwearIcon, headwear);
        float textX = iconX + IconSize + 10f;

        if (costume.bodyPiece != null)
        {
            Rect bodyIcon = new(iconX + IconSize + 6f, rect.y + 8f, IconSize, IconSize);
            Widgets.DefIcon(bodyIcon, costume.bodyPiece);
            AddItemTooltip(bodyIcon, costume.bodyPiece);
            textX += IconSize + 6f;
        }

        float textWidth = rect.xMax - textX - 4f;
        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.MiddleLeft;
        float lineHeight = Mathf.Max(18f, Mathf.Ceil(Text.LineHeight) + 2f);
        Rect line = new(textX, rect.y + 3f, textWidth, lineHeight);
        Widgets.Label(line, "OMW_CostumeHeadwearLine".Translate(headwear.LabelCap, ModName(headwear)));
        AddItemTooltip(line, headwear);
        line.y += lineHeight;

        if (costume.bodyPiece != null)
        {
            Widgets.Label(line, "OMW_CostumeBodyLine".Translate(costume.bodyPiece.LabelCap, ModName(costume.bodyPiece)));
            AddItemTooltip(line, costume.bodyPiece);
        }
        else
            Widgets.Label(line, "OMW_CostumeNoBodyLine".Translate());
        line.y += lineHeight;

        string stages = row.forAdults && row.forChildren
            ? "OMW_CostumeStageBoth".Translate().ToString()
            : row.forChildren
                ? "OMW_CostumeStageChild".Translate().ToString()
                : "OMW_CostumeStageAdult".Translate().ToString();
        Widgets.Label(line, "OMW_CostumeStageLine".Translate(stages));

        if (!CoversFullHead(costume))
        {
            GUI.color = new Color(1f, 0.62f, 0.28f);
            Rect warning = new(textX, rect.y + rect.height - lineHeight - 3f, textWidth, lineHeight);
            Widgets.Label(warning, "OMW_CostumeFullHeadWarning".Translate());
            GUI.color = Color.white;
        }

        Widgets.DrawLine(new Vector2(rect.x, rect.yMax - 1f), new Vector2(rect.xMax, rect.yMax - 1f), new Color(0.35f, 0.35f, 0.35f), 1f);
    }

    private static void AddItemTooltip(Rect rect, ThingDef def)
    {
        string text = def.LabelCap.ToString() + "\n\n" + def.DescriptionDetailed;
        TooltipHandler.TipRegion(rect, new TipSignal(text, def.GetHashCode()));
    }

    private static bool CoversFullHead(SamhainCostumeSet costume)
    {
        return CoversFullHead(costume.headwear) || CoversFullHead(costume.bodyPiece);
    }

    private static bool CoversFullHead(ThingDef? def)
    {
        if (def?.apparel?.bodyPartGroups == null)
            return false;

        foreach (BodyPartGroupDef group in def.apparel.bodyPartGroups)
        {
            if (group.defName == "FullHead")
                return true;
        }
        return false;
    }

    private static string ModName(ThingDef def)
    {
        return def.modContentPack?.Name ?? "OMW_UnknownMod".Translate().ToString();
    }
}
