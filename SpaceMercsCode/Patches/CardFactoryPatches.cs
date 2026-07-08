using HarmonyLib;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Enums;

namespace SpaceMercs.SpaceMercsCode.Patches;

[HarmonyPatch(typeof(CardFactory), nameof(CardFactory.GetFilteredTransformationOptions))]
public class CardFactoryPatch
{
    [HarmonyPostfix]
    private static void Postfix(ref CardModel[] __result)
    {
        List<CardModel> newResult = new();
        foreach (var card in __result)
        {
            if (!card.Tags.Contains(SpaceMercsTags.NonTransformPool))
            {
                newResult.Add(card);
            }
        }
        
        __result = newResult.ToArray();
    }
}