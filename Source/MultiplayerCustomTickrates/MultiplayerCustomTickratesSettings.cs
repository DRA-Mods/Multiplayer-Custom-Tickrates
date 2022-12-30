using System;
using Multiplayer.API;
using Multiplayer.Client;
using UnityEngine;
using Verse;
using static MultiplayerCustomTickrates.TickRateReplacements;

namespace MultiplayerCustomTickrates;

public class MultiplayerCustomTickratesSettings : ModSettings
{
    public int targetTicksNormal = 60;
    public int targetTicksFast = 180;
    public int targetTicksSuperfast = 360;
    public int targetTicksSuperfastNothingHappening = 720;
    public int targetTicksUltrafast = 900;

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref targetTicksNormal, "targetTicksNormal", 60);
        Scribe_Values.Look(ref targetTicksFast, "targetTicksFast", 180);
        Scribe_Values.Look(ref targetTicksSuperfast, "targetTicksSuperfast", 360);
        Scribe_Values.Look(ref targetTicksSuperfastNothingHappening, "targetTicksSuperfastNothingHappening", 720);
        Scribe_Values.Look(ref targetTicksUltrafast, "targetTicksUltrafast", 900);
    }

    public void DoSettingsWindowContents(Rect inRect)
    {
        var listing = new Listing_Standard();
        listing.Begin(inRect);
        listing.ColumnWidth = 270f;

        listing.Label("MpTickratesWarnSameValue".Translate().CapitalizeFirst());
        listing.Label("MpTickratesWarnNormalSpeed".Translate().CapitalizeFirst());

        using (new TextBlock(TextAnchor.MiddleRight))
        {
            DrawGameSpeedTarget(listing, "MpTickratesTargetNormal", ref targetTicksNormal, 1, 9999);
            DrawGameSpeedTarget(listing, "MpTickratesTargetFast", ref targetTicksFast, 1, 9999);
            DrawGameSpeedTarget(listing, "MpTickratesTargetSuperfast", ref targetTicksSuperfast, 1, 9999);
            DrawGameSpeedTarget(listing, "MpTickratesTargetSuperfastNothingHappening", "MpTickratesTargetSuperfastNothingHappeningTip", ref targetTicksSuperfastNothingHappening, 1, 9999);
            DrawGameSpeedTarget(listing, "MpTickratesTargetUltrafast", "MpTickratesTargetUltrafastTip", ref targetTicksUltrafast, 1, 9999);
        }

        if ((targetTicksNormal != 60 ||
            targetTicksFast != 180 ||
            targetTicksSuperfast != 360 ||
            targetTicksSuperfastNothingHappening != 720 ||
            targetTicksUltrafast != 900) &&
            listing.ButtonText("MpTickratesResetDefault".Translate().CapitalizeFirst()))
        {
            targetTicksNormal = 60;
            targetTicksFast = 180;
            targetTicksSuperfast = 360;
            targetTicksSuperfastNothingHappening = 720;
            targetTicksUltrafast = 900;
        }

        if (MP.IsInMultiplayer &&
            MP.IsHosting &&
            Math.Abs(tickRateNormal / 60f - targetTicksNormal) > Mathf.Epsilon &&
            Math.Abs(tickRateFast / 60f - targetTicksFast) > Mathf.Epsilon &&
            Math.Abs(tickRateSuperfast / 60f - targetTicksSuperfast) > Mathf.Epsilon &&
            Math.Abs(tickRateUltrafast / 60f - targetTicksSuperfastNothingHappening) > Mathf.Epsilon &&
            Math.Abs(tickRateSuperfastNothingHappening / 60f - targetTicksUltrafast) > Mathf.Epsilon &&
            listing.ButtonText("MpTickratesApply".Translate().CapitalizeFirst()))
        {
            ApplyChanges(targetTicksNormal, targetTicksFast, targetTicksSuperfast, targetTicksSuperfastNothingHappening, targetTicksUltrafast);
        }

        listing.End();
    }

    private static void DrawGameSpeedTarget(Listing listing, string label, ref int currentValue, int min, int max)
        => DrawGameSpeedTarget(listing, label, null, ref currentValue, min, max);

    private static void DrawGameSpeedTarget(Listing listing, string label, string tip, ref int currentValue, int min, int max)
    {
        var rect = listing.GetRect(30);
        var labelRect = rect.Width(240);
        Widgets.Label(labelRect, $"{label.Translate().CapitalizeFirst()}:");
        if (tip != null && Mouse.IsOver(labelRect))
            TooltipHandler.TipRegion(labelRect, new TipSignal(tip.Translate().CapitalizeFirst(), 1531));

        rect = rect.Right(250);

        string buffer = null;
        Widgets.TextFieldNumeric(
            rect.Width(50f),
            ref currentValue,
            ref buffer,
            min,
            max
        );

        rect = rect.Right(56F);
        var ticksText = "MpSpeedTargetTicks".Translate();
        Widgets.Label(rect.Width(ticksText.GetWidthCached()), ticksText);
    }

    [SyncMethod]
    public static void ApplyChanges(int targetTicksNormal, int targetTicksFast, int targetTicksSuperfast, int targetTicksSuperfastNothingHappening, int targetTicksUltrafast)
    {
        tickRateNormal = targetTicksNormal / 60f;
        tickRateFast = targetTicksFast / 60f;
        tickRateSuperfast = targetTicksSuperfast / 60f;
        tickRateUltrafast = targetTicksSuperfastNothingHappening / 60f;
        tickRateSuperfastNothingHappening = targetTicksUltrafast / 60f;
    }
}