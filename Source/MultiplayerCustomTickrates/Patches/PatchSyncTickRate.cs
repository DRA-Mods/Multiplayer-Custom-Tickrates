using HarmonyLib;
using Multiplayer.Client.Comp;
using Verse;
using static MultiplayerCustomTickrates.TickRateReplacements;

namespace MultiplayerCustomTickrates.Patches;

[HarmonyPatch(typeof(MultiplayerGameComp), nameof(MultiplayerGameComp.ExposeData))]
public static class PatchSyncTickRate
{
    public static void Postfix()
    {
        Scribe_Values.Look(ref tickRateNormal, "tickRateNormal", 1f);
        Scribe_Values.Look(ref tickRateFast, "tickRateFast", 3f);
        Scribe_Values.Look(ref tickRateSuperfast, "tickRateSuperfast", 6f);
        Scribe_Values.Look(ref tickRateUltrafast, "tickRateUltrafast", 15f);
        Scribe_Values.Look(ref tickRateSuperfastNothingHappening, "tickRateSuperfastNothingHappening", 12f);
    }
}