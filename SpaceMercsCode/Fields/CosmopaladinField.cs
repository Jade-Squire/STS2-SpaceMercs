using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using SpaceMercs.SpaceMercsCode.CombatState;

namespace SpaceMercs.SpaceMercsCode.Fields;

/*******************************************************************************************************************
 * Credit to Blizzarre's Runesmith2 for this custom alt energy cost (https://github.com/Blizzarre/Runesmith2-StS2) *
 *******************************************************************************************************************/

public static class CosmopaladinField
{
    public static readonly SpireField<PlayerCombatState, PlayerCombatStateExtensions.CosmopaladinCombatState>
        CosmopaladinCombatState = new(() => null);
}