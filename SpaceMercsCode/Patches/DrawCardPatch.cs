using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Extensions;
using BaseLib.Utils.Patching;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Combat;
using SpaceMercs.SpaceMercsCode.Hooks;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Patches;

/*[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw))]
[HarmonyPatch(MethodType.Async)]
[HarmonyPatch(new Type[] { typeof(PlayerChoiceContext), typeof(Decimal), typeof(Player), typeof(bool) })]
public class DrawCardPatch
{
    [HarmonyPrefix]
    static async Task<IEnumerable<CardModel>> Prefix(PlayerChoiceContext ___choiceContext, Player ___player, Decimal ___count, bool ___fromHandDraw)
    {
        if (___player.Creature.HasPower<BlindedPower>())
        {
            return await ___player.Creature.GetPower<BlindedPower>().DrawCard(___choiceContext, ___player, ___count, ___fromHandDraw);
        }
        return Enumerable.Empty<CardModel>();
    }
}*/

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw))]
[HarmonyPatch(MethodType.Async)]
[HarmonyPatch(new Type[] { typeof(PlayerChoiceContext), typeof(Decimal), typeof(Player), typeof(bool) })]
public class DrawCardPatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
    {
        FieldInfo player = originalMethod.DeclaringType.GetField("player");
        FieldInfo hand = AccessTools.Field(AccessTools.TypeByName("<Draw>d__16"), "<hand>5__4");
        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .ldarg_0()
            .ldc_i4_2()
            .ldarg_0()
            .opcode(OpCodes.Ldfld)
            .opcode(OpCodes.Call)).Insert([
            CodeInstruction.LoadArgument(0),
            new CodeInstruction(OpCodes.Ldfld, player),
            CodeInstruction.Call(typeof(DrawCardPatch), nameof(ChangePileToDrawTo))]);

        /*foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Stfld && (FieldInfo)instruction.operand == hand)
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, player);
                yield return new CodeInstruction(OpCodes.Call, ChangePileToDrawTo);
            }
            
            yield return instruction;
        }*/
    }

    private static CardPile ChangePileToDrawTo(CardPile hand, Player player)
    {
        if (player.Creature.HasPower<BlindedPower>())
        {
            hand = SpacemercsCustomPile.Aether.GetPile(player);
        }

        return hand;
    }
}