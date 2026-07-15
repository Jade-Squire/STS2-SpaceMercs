using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using SpaceMercs.SpaceMercsCode.Combat;

namespace SpaceMercs.SpaceMercsCode.Patches;

[HarmonyPatch(typeof(NCardFlyShuffleVfx), nameof(NCardFlyShuffleVfx.Create))]
public class NCardFlyShuffleVfxPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref NCardFlyShuffleVfx? __result) 
    {
        if (__result != null && __result._targetPile.Type == SpacemercsCustomPile.Aether)
        {
            __result = null;
        }
    }
}