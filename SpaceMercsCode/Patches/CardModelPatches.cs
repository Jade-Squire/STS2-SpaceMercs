using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Patches;

/*******************************************************************************************************************
 * Credit to Blizzarre's Runesmith2 for this custom alt energy cost (https://github.com/Blizzarre/Runesmith2-StS2) *
 *******************************************************************************************************************/

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
class SpendResourcesPatch
{
    static async Task<(int, int)> Postfix(Task<(int, int)> results, CardModel __instance)
    {
        var ret = await results;
        if (__instance is not SpaceMercsCard card) return ret;
        var DeterminationToSpend = Math.Max(0, card.GetDeterminationCostWithModifiers());
        await card.SpendDetermination(DeterminationToSpend);
        return ret;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SetToFreeThisCombat))]
class SetToFreeThisCombatPatch
{
    [HarmonyPostfix]
    static void Postfix(CardModel __instance)
    {
        if (__instance is SpaceMercsCard)
        {
            ((SpaceMercsCard)__instance).SetDeterminationCostThisCombat(0);
        }
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SetToFreeThisTurn))]
class SetToFreeThisTurnPatch
{
    [HarmonyPostfix]
    static void Postfix(CardModel __instance)
    {
        if (__instance is SpaceMercsCard)
        {
            ((SpaceMercsCard)__instance).SetDeterminationCostThisTurn(0);
        }
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.CanPlay),
    [typeof(UnplayableReason), typeof(AbstractModel)], [ArgumentType.Out, ArgumentType.Out])]
internal class CardModelCanPlayPatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .opcode(OpCodes.Or)
            .opcode(OpCodes.Stind_I4)
            .ldarg_1()
            .opcode(OpCodes.Ldind_I4)
            .opcode(OpCodes.Ldc_I4_0)
            .opcode(OpCodes.Ceq)
        ).Step(-3).Insert([
            CodeInstruction.LoadArgument(0),
            CodeInstruction.LoadArgument(1),
            new CodeInstruction(OpCodes.Ldind_I4),
            CodeInstruction.LoadArgument(2),
            CodeInstruction.Call(typeof(CardModelCanPlayPatch), nameof(GetPreventerModel))
        ]);
    }
    private static void GetPreventerModel(CardModel card, UnplayableReason reason, ref AbstractModel? preventer)
    {
        if (reason == UnplayableReason.None) return;
        if (card is not SpaceMercsCard spaceMercsCard) return;
        if (reason.HasFlag(UnplayableReason.StarCostTooHigh)) preventer = spaceMercsCard;
    }
}
