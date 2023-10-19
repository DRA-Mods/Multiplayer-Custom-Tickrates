using HarmonyLib;
using Multiplayer.Client;
using static MultiplayerCustomTickrates.TickRateReplacements;

namespace MultiplayerCustomTickrates.Patches;

[HarmonyPatch(typeof(HostUtil), nameof(HostUtil.HostServer))]
public static class PatchInitTickRate
{
    public static void Postfix()
    {
        tickRateNormal = MultiplayerCustomTickratesMod.settings.targetTicksNormal / 60f;
        tickRateFast = MultiplayerCustomTickratesMod.settings.targetTicksFast / 60f;
        tickRateSuperfast = MultiplayerCustomTickratesMod.settings.targetTicksSuperfast / 60f;
        tickRateUltrafast = MultiplayerCustomTickratesMod.settings.targetTicksSuperfastNothingHappening / 60f;
        tickRateSuperfastNothingHappening = MultiplayerCustomTickratesMod.settings.targetTicksUltrafast / 60f;
    }
}