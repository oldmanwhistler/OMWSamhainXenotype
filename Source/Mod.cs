using RimWorld;
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
        {
            const float buttonHeight = 34f;
            const float footerGap = 8f;
            Rect costumeList = new(contents.x, contents.y, contents.width, contents.height - buttonHeight - footerGap);
            SamhainCostumeSettingsPage.Draw(costumeList, Settings, ref settingsScrollPosition, WriteSettings);

            Rect footer = new(contents.x, contents.yMax - buttonHeight, contents.width, buttonHeight);
            float buttonWidth = 120f;
            Rect saveButton = new(footer.x, footer.y, buttonWidth, buttonHeight);
            Rect resetButton = new(footer.x + buttonWidth + 8f, footer.y, buttonWidth, buttonHeight);
            if (Widgets.ButtonText(saveButton, "OMW_CostumeSettingsSave".Translate()))
            {
                WriteSettings();
                Messages.Message("OMW_CostumeSettingsSaved".Translate(), MessageTypeDefOf.PositiveEvent, historical: false);
            }
            if (Widgets.ButtonText(resetButton, "OMW_CostumeSettingsReset".Translate()))
            {
                Settings.ResetCostumeChoices();
                WriteSettings();
                settingsScrollPosition = Vector2.zero;
                Messages.Message("OMW_CostumeSettingsResetDone".Translate(), MessageTypeDefOf.PositiveEvent, historical: false);
            }
        }
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
            Log.Message($"[OMW] Costume assignment logging {(enabled ? "enabled" : "disabled")}.");
        }
        Widgets.Label(new Rect(inRect.x, inRect.y + 36f, inRect.width, 44f), "OMW_LogCostumeAssignmentsDescription".Translate());
    }
}
