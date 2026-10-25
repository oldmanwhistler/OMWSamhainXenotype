using UnityEngine;
using Verse;

namespace SamhainXenotype;

public class SamhainXenotypeMod : Mod
{
    private Vector2 settingsScrollPosition;

    public SamhainXenotypeMod(ModContentPack content) : base(content)
    {
        Log.Message($"[{content.Name}] loaded.");
    }

    public SamhainXenotypeSettings Settings => GetSettings<SamhainXenotypeSettings>();

    public static SamhainXenotypeSettings? ActiveSettings => LoadedModManager.GetMod<SamhainXenotypeMod>()?.Settings;

    public override string SettingsCategory() => "OMW_SamhainSettingsCategory".Translate();

    public override void DoSettingsWindowContents(Rect inRect)
    {
        SamhainCostumeSettingsPage.Draw(inRect, Settings, ref settingsScrollPosition, WriteSettings);
    }
}
