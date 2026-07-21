using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Character;

namespace SpaceMercs.SpaceMercsCode.Patches;

[HarmonyPatch(typeof(ArchaicTooth), nameof(ArchaicTooth.UpdateHoverTips))]
public class ArchaicToothUpdateHoverTipsPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ArchaicTooth __instance)
    {
        if (__instance.StarterCard != null && __instance.AncientCard != null)
        {
            CardModel card = CardModel.FromSerializable(__instance.StarterCard);
            if (card is not RememberedVow && card is not BrokenOath)
            {
                return true;
            }
            __instance._extraHoverTips.Clear();
            __instance._extraHoverTips.Add(HoverTipFactory.FromCard(card));
            ((StringVar)__instance.DynamicVars["StarterCard"]).StringValue = card.Title;
            card = CardModel.FromSerializable(__instance.AncientCard);
            __instance._extraHoverTips.AddRange(card.HoverTips);
            __instance._extraHoverTips.Add(HoverTipFactory.FromCard(card));
            ((StringVar)__instance.DynamicVars["AncientCard"]).StringValue = card.Title;
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(ArchaicTooth), nameof(ArchaicTooth.GetTranscendenceTransformedCard))]
public class ArchaicToothGetTranscendenceTransformedCardPatch
{
    [HarmonyPostfix]
    public static void Postfix(ArchaicTooth __instance, ref CardModel __result, CardModel starterCard)
    {
        if (starterCard is RememberedVow)
        {
            ((KeptPromise)__result).CurrentBlock = ((RememberedVow)starterCard).CurrentBlock;
            ((KeptPromise)__result).IncreasedBlock = ((RememberedVow)starterCard).IncreasedBlock;
            ((KeptPromise)__result).CurrentDamage = ((RememberedVow)starterCard).CurrentDamage;
            ((KeptPromise)__result).IncreasedDamage = ((RememberedVow)starterCard).IncreasedDamage;
        }
    }
}

[HarmonyPatch(typeof(ArchaicTooth), nameof(ArchaicTooth.SetupForPlayer))]
public class ArchaicToothSetupForPlayerPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ArchaicTooth __instance, Player player, ref bool __result)
    {
        if (player.Character is Cosmopaladin)
        {
            bool foundVow = false;
            bool foundOath = false;
            CardModel? vow = null;
            CardModel? oath = null;
            foreach (var card in player.Deck.Cards)
            {
                if (card is BrokenOath)
                {
                    foundOath = true;
                    oath = card;
                    if (foundVow)
                    {
                        break;
                    }
                }

                if (card is RememberedVow)
                {
                    foundVow = true;
                    vow = card;
                    if (foundOath)
                    {
                        break;
                    }
                }
            }

            if (foundVow && foundOath)
            {
                __result = false;
                return false;
            }

            if (foundVow)
            {
                __instance.StarterCard = vow.ToSerializable();
                __instance.AncientCard = __instance.GetTranscendenceTransformedCard(vow).ToSerializable();
                __result = true;
                __instance.UpdateHoverTips();
                return false;
            }

            if (foundOath)
            {
                __instance.StarterCard = oath.ToSerializable();
                __instance.AncientCard = __instance.GetTranscendenceTransformedCard(oath).ToSerializable();
                __result = true;
                __instance.UpdateHoverTips();
                return false;
            }
        }

        return true;
    }
}