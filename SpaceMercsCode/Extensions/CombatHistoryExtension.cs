using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Players;
using SpaceMercs.SpaceMercsCode.Combat;

namespace SpaceMercs.SpaceMercsCode.Extensions;

public static class CombatHistoryExtension
{
    public static void DeterminationModified(this CombatHistory combatHistory, ICombatState combatState, int amount,
        Player player)
    {
        combatHistory.Add(combatState, new DeterminationModifiedEntry(amount, player, combatState.RoundNumber, combatState.CurrentSide, combatHistory, [player]));
    }
}