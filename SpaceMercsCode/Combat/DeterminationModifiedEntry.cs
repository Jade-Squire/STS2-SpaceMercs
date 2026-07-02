using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Players;

namespace SpaceMercs.SpaceMercsCode.Combat;

public class DeterminationModifiedEntry (int amount, Player player, int roundNumber, CombatSide currentSide, CombatHistory history, IEnumerable<Player> players) : CombatHistoryEntry(player.Creature, roundNumber, currentSide, history, players)
{
    public int Amount { get; } = amount;
    public Player Player { get; } = player;

    public override string Description
    {
        get
        {
            var left = $"{Actor.Player?.Character.Id.Entry} {(Amount < 0 ? "lost" : "gained")} ";

            string[] arr =
            [
                $"{Amount} Determination"
            ];

            return $"{left} {string.Join(", ", arr.Where(s => !string.IsNullOrEmpty(s)))}";
        }
    }
}