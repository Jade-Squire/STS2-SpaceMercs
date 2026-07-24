using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Relics;

namespace SpaceMercs.SpaceMercsCode.Patches;

[HarmonyPatch (typeof(TouchOfOrobas), nameof(TouchOfOrobas.SetupForPlayer))]
public class SetupForPlayerPatch
{
    [HarmonyPrefix]
    public static bool Prefix(TouchOfOrobas __instance, Player player, ref bool __result)
    {
        __instance.AssertMutable();
        RelicModel starterRelic = __instance.GetStarterRelic(player);
        if (starterRelic == null)
            return true;
        if (player.Character is Cosmopaladin && starterRelic is AutoResponsiveCuirass cuirass)
        {
            __instance.StarterRelic = starterRelic.Id;
            switch (cuirass.Subclass)
            {
                case Subclasses.Solar:
                    __instance.UpgradedRelic = ModelDb.Relic<Fireball>().Id;
                    break;
                case Subclasses.Arc:
                    __instance.UpgradedRelic = ModelDb.Relic<LightningInABottle>().Id;
                    break;
                case Subclasses.Void:
                    __instance.UpgradedRelic = ModelDb.Relic<ShiningFork>().Id;
                    break;
                case Subclasses.Stasis:
                    __instance.UpgradedRelic = ModelDb.Relic<FrozenCoat>().Id;
                    break;
                case Subclasses.Lightless:
                default:
                    __instance.UpgradedRelic = ModelDb.Relic<EnergizedCuirass>().Id;
                    break;
            }

            __result = true;
            
            return false;
        }
        
        return true;
    }
}