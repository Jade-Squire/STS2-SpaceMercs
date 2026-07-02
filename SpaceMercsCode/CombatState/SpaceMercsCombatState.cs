using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using SpaceMercs.SpaceMercsCode.Commands;
using SpaceMercs.SpaceMercsCode.Extensions;
using SpaceMercs.SpaceMercsCode.Fields;
using SpaceMercs.SpaceMercsCode.Nodes;

namespace SpaceMercs.SpaceMercsCode.CombatState;

public static class PlayerCombatStateExtensions
{
    public class CosmopaladinCombatState(PlayerCombatState combatState)
    {
        public event Action<int, int>? DeterminationChanged;

        public int Determination
        {
            get;
            private set
            {
                if (field == value) return;
                int determination = field;
                field = value;
                var state = combatState._player.Creature.CombatState;
                if (state != null)
                {
                    CombatManager.Instance.History.DeterminationModified(state, field - determination, combatState._player);
                }
                Action<int, int> determinationChanged = DeterminationChanged;
                if (determinationChanged == null)
                    return;
                determinationChanged(determination, field);
            }
        }

        public void LoseDetermination(decimal amount)
        {
            Determination = !(amount < 0m)
                ? (int)Math.Clamp(Determination - amount, 0, 99999999)
                : throw new ArgumentException("Must not be negative.", nameof(amount));
        }
        
        public void GainDetermination(decimal amount)
        {
            Determination = !(amount < 0m)
                ? (int)Math.Clamp(Determination + amount, 0, 99999999)
                : throw new ArgumentException("Must not be negative.", nameof(amount));
        }
    }

    extension(PlayerCombatState combatState)
    {
        public int GetDetermination()
        {
            var cosmopaladinCombatState = combatState.Cosmopaladin();
            return cosmopaladinCombatState?.Determination ?? 0;
        }

        public CosmopaladinCombatState? Cosmopaladin()
        {
            return CosmopaladinField.CosmopaladinCombatState[combatState];
        }
    }
}