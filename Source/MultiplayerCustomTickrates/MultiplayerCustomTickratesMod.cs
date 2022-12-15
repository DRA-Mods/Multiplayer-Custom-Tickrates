using HarmonyLib;
using Multiplayer.API;
using UnityEngine;
using Verse;

namespace MultiplayerCustomTickrates;

public class MultiplayerCustomTickratesMod : Mod
{
    private static Harmony harmony;
    internal static Harmony Harmony => harmony ??= new Harmony("Dra.MultiplayerCustomTickratesMod");
    public static MultiplayerCustomTickratesSettings settings;

    public MultiplayerCustomTickratesMod(ModContentPack content) : base(content)
    {
        settings = GetSettings<MultiplayerCustomTickratesSettings>();

        if (!MP.enabled)
        {
            Log.Error("[Multiplayer - Custom Tickrates] Multiplayer is not enabled, having this mod enabled is completely pointless.");
            return;
        }

        MP.RegisterAll();
        Harmony.PatchAll();
        AddIgnoredConfigModId(content.ModMetaData.PackageIdNonUnique);
    }

    private static void AddIgnoredConfigModId(string targetId)
    {
        const string warning = "[Multiplayer - Custom Tickrates] couldn't add the mod to ignored mod configs for syncing";
        const string targetField = "Multiplayer.Client.JoinData:ignoredConfigsModIds";
        
        var ignoredConfigsField = AccessTools.Field(targetField);

        if (ignoredConfigsField == null)
        {
            Log.Warning($"{warning} - couldn't find field ({targetField})");
            return;
        }

        if (!ignoredConfigsField.IsStatic)
        {
            Log.Warning($"{warning} - target field is not static");
            return;
        }

        var value = ignoredConfigsField.GetValue(null);

        if (value is not string[] configs)
        {
            Log.Warning($"{warning} - target field did not return string array type - actual value ({value.ToStringSafe()}) of type {ignoredConfigsField.FieldType}");
            return;
        }

        ignoredConfigsField.SetValue(null, configs.AddToArray(targetId));
    }

    public override void DoSettingsWindowContents(Rect inRect) => settings.DoSettingsWindowContents(inRect);

    public override string SettingsCategory() => "MP - Configurable Tickrates";
}