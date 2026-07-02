using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SpaceMercs.SpaceMercsCode.Character;

namespace SpaceMercs.SpaceMercsCode.Patches;

[HarmonyPatch(typeof(NEnergyCounter), nameof(NEnergyCounter.Create))]
public class NEnergyCounterPatch
{
    [HarmonyTranspiler]
    public static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .ldloc_0()
            .ldarg_0()
            .opcode(OpCodes.Stfld)
        ).Insert([
            CodeInstruction.LoadLocal(0),
            CodeInstruction.LoadArgument(0),
            CodeInstruction.Call(typeof(NEnergyCounterPatch), nameof(SetPlayer))
        ]);
    }

    static void SetPlayer(NEnergyCounter energyCounter, Player player)
    {
        AddedNodes.AddedNodes.NDeterminationCounter[energyCounter].SetPlayer(player);
    }
}