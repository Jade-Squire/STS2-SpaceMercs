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

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw))]
[HarmonyPatch(MethodType.Async)]
[HarmonyPatch(new Type[] { typeof(PlayerChoiceContext), typeof(Decimal), typeof(Player), typeof(bool) })]
public class DrawCardPatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
    {
        FieldInfo player = originalMethod.DeclaringType.GetField("player");
        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .ldarg_0()
            .ldc_i4_2()
            .ldarg_0()
            .opcode(OpCodes.Ldfld)
            .opcode(OpCodes.Call)).Insert([
            CodeInstruction.LoadArgument(0),
            new CodeInstruction(OpCodes.Ldfld, player),
            CodeInstruction.Call(typeof(DrawCardPatch), nameof(ChangePileToDrawTo))]);
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