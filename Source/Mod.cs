using UnityEngine;
using Verse;

namespace SamhainXenotype;

public class SamhainXenotypeMod : Mod
{
    private Vector2 settingsScrollPosition;
    private int selectedSettingsTab;

    public SamhainXenotypeMod(ModContentPack content) : base(content)
    {
        Log.Message($"[{content.Name}] loaded.");
    }

    public SamhainXenotypeSettings Settings => GetSettings<SamhainXenotypeSettings>();

    public static SamhainXenotypeSettings? ActiveSettings => LoadedModManager.GetMod<SamhainXenotypeMod>()?.Settings;

    public override string SettingsCategory() => "OMW_SamhainSettingsCategory".Translate();

    public override void DoSettingsWindowContents(Rect inRect)
    {
        const float tabHeight = 34f;
        float tabWidth = inRect.width / 2f;
        Rect costumesTab = new(inRect.x, inRect.y, tabWidth, tabHeight);
        Rect loggingTab = new(inRect.x + tabWidth, inRect.y, tabWidth, tabHeight);

        if (selectedSettingsTab == 0)
            Widgets.DrawHighlight(costumesTab);
        if (Widgets.ButtonText(costumesTab, "OMW_SettingsTabCostumes".Translate()))
            selectedSettingsTab = 0;

        if (selectedSettingsTab == 1)
            Widgets.DrawHighlight(loggingTab);
        if (Widgets.ButtonText(loggingTab, "OMW_SettingsTabLogging".Translate()))
            selectedSettingsTab = 1;

        Rect contents = new(inRect.x, inRect.y + tabHeight + 6f, inRect.width, inRect.height - tabHeight - 6f);
        if (selectedSettingsTab == 0)
            SamhainCostumeSettingsPage.Draw(contents, Settings, ref settingsScrollPosition, WriteSettings);
        else
            DrawLoggingSettings(contents);
    }

    private void DrawLoggingSettings(Rect inRect)
    {
        bool enabled = Settings.LogCostumeAssignments;
        Widgets.CheckboxLabeled(inRect.TopPartPixels(30f), "OMW_LogCostumeAssignments".Translate(), ref enabled);
        if (enabled != Settings.LogCostumeAssignments)
        {
            Settings.LogCostumeAssignments = enabled;
            WriteSettings();
        }
        Widgets.Label(new Rect(inRect.x, inRect.y + 36f, inRect.width, 44f), "OMW_LogCostumeAssignmentsDescription".Translate());
    }
}
