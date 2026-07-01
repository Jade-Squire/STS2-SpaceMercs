using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.CombatState;
using SpaceMercs.SpaceMercsCode.Fields;

namespace SpaceMercs.SpaceMercsCode.Patches;

/*******************************************************************************************************************
 * Credit to Blizzarre's Runesmith2 for this custom alt energy cost (https://github.com/Blizzarre/Runesmith2-StS2) *
 *******************************************************************************************************************/

[HarmonyPatch(typeof(PlayerCombatState), MethodType.Constructor)]
[HarmonyPatch([typeof(Player)])]
public static class PlayerCombatStateConstructorPatch
{
    [HarmonyPostfix]
    private static void Postfix(Player player, PlayerCombatState __instance)
    {
        var cosmopaladinCombatState = new PlayerCombatStateExtensions.CosmopaladinCombatState(__instance);
        CosmopaladinField.CosmopaladinCombatState[__instance] = cosmopaladinCombatState;
    }
}


[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
public static class HasEnoughEnergyForPatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> ret = new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .ldarg_2()
            .opcode(OpCodes.Ldc_I4_0)
            .opcode(OpCodes.Stind_I4)
        ).Insert([
            CodeInstruction.LoadArgument(0),
            CodeInstruction.LoadArgument(1),
            CodeInstruction.LoadArgument(2),
            CodeInstruction.Call(typeof(HasEnoughEnergyForPatch), nameof(HasEnoughDetermination))
        ]);

        return ret;
    }

    static void HasEnoughDetermination(PlayerCombatState instance, CardModel card, ref UnplayableReason reason)
    {
        if (card is not SpaceMercsCard c) return;
        if (c.CurrentDeterminationCost <= 0) return;
        if (instance.GetDetermination() < c.CurrentDeterminationCost)
        {
            reason |= UnplayableReason.StarCostTooHigh;
        }
    }
}