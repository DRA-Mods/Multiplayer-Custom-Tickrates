using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Multiplayer.Client;
using UnityEngine;

namespace MultiplayerCustomTickrates.Patches;

[HarmonyPatch]
internal static class PatchTickRate
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(AsyncTimeComp), nameof(AsyncTimeComp.TickRateMultiplier));
        yield return AccessTools.Method(typeof(MultiplayerWorldComp), nameof(MultiplayerWorldComp.TickRateMultiplier));
        yield return AccessTools.Method(typeof(TickRatePatch), "Prefix");
    }

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
    {
        var codeInstructions = instr.ToArray();

        for (var index = 0; index < codeInstructions.Length; index++)
        {
            var ci = codeInstructions[index];
            var value = -1;
            var isInt = true;
            var isSbyte = false;
            var nextInstr = index + 1 < codeInstructions.Length ? codeInstructions[index + 1] : null;

            if (nextInstr == null || nextInstr.opcode == OpCodes.Br || nextInstr.opcode == OpCodes.Br_S || !nextInstr.Branches(out _))
            {
                if (ci.opcode == OpCodes.Ldc_I4_1)
                    value = 1;
                else if (ci.opcode == OpCodes.Ldc_I4_3)
                    value = 3;
                else if (ci.opcode == OpCodes.Ldc_I4_6)
                    value = 6;
                else if (ci.opcode == OpCodes.Ldc_I4_S && ci.operand is sbyte sbyteValue)
                {
                    value = sbyteValue;
                    isSbyte = true;
                }
                else if (ci.opcode == OpCodes.Ldc_I4 && ci.operand is int intValue)
                    value = intValue;
                else if (ci.opcode == OpCodes.Ldc_R4 && ci.operand is float floatVal)
                {
                    value = Mathf.RoundToInt(floatVal);
                    isInt = false;
                }
            }

            if (value > 0)
            {
                var target = value switch
                {
                    1 => AccessTools.Field(typeof(TickRateReplacements), nameof(TickRateReplacements.tickRateNormal)),
                    3 => AccessTools.Field(typeof(TickRateReplacements), nameof(TickRateReplacements.tickRateFast)),
                    6 => AccessTools.Field(typeof(TickRateReplacements), nameof(TickRateReplacements.tickRateSuperfast)),
                    12 => AccessTools.Field(typeof(TickRateReplacements), nameof(TickRateReplacements.tickRateUltrafast)),
                    15 => AccessTools.Field(typeof(TickRateReplacements), nameof(TickRateReplacements.tickRateSuperfastNothingHappening)),
                    _ => null,
                };

                if (target != null)
                {
                    ci.opcode = OpCodes.Ldsfld;
                    ci.operand = target;
                }

                yield return ci;

                if (isSbyte)
                {
                    if (nextInstr != null && nextInstr.opcode == OpCodes.Conv_R4)
                        index++;
                    else
                        yield return new CodeInstruction(OpCodes.Conv_I1);
                    
                }
                else if (isInt)
                {
                    if (nextInstr != null && nextInstr.opcode == OpCodes.Conv_R4)
                        index++;
                    else
                        yield return new CodeInstruction(OpCodes.Conv_I4);
                }
            }
            else yield return ci;
        }
    }
}