using HarmonyLib;
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
