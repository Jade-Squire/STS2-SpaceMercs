using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using SpaceMercs.SpaceMercsCode.CombatState;

namespace SpaceMercs.SpaceMercsCode.Commands;

public static class CosmopaladinPlayerCmd
{
    public static async Task GainDetermination(int amount, Player player, CardPlay? cardPlay = null)
    {
        if (amount > 0 && !CombatManager.Instance.IsEnding && player.Creature.CombatState != null)
        {
            var combatState = player.Creature.CombatState;
            var cosmoCombatState = player.PlayerCombatState?.Cosmopaladin();
            cosmoCombatState?.GainDetermination(amount);
        }
    }

    public static Task LoseDetermination(int amount, Player player)
    {
        if (amount <= 0 || CombatManager.Instance.IsEnding) return Task.CompletedTask;
        
        player.PlayerCombatState?.Cosmopaladin()?.LoseDetermination(amount);
        return Task.CompletedTask;
    }

    public static async Task ResetDetermination(Player player)
    {
        if (!CombatManager.Instance.IsEnding)
        {
            int determination = player.PlayerCombatState?.Cosmopaladin()?.Determination ?? 0;
            await LoseDetermination(determination, player);
        }
    }
}