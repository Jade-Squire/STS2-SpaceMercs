using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Patches;

[HarmonyPatch(typeof(UnplayableReasonExtensions), nameof(UnplayableReasonExtensions.GetPlayerDialogueLine))]
public class UnplayableReasonPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref LocString? __result, UnplayableReason reason, AbstractModel? preventer)
    {
        if (reason.HasFlag(UnplayableReason.StarCostTooHigh) && preventer is SpaceMercsCard)
        {
            __result = new LocString("combat_messages", "SPACEMERCS-NOT_ENOUGH_DETERMINATION");
            return false;
        }

        return true;
    }
}