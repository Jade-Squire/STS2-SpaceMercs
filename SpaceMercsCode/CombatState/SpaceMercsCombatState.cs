using MegaCrit.Sts2.Core.Entities.Players;
using SpaceMercs.SpaceMercsCode.Fields;

namespace SpaceMercs.SpaceMercsCode.CombatState;

public static class PlayerCombatStateExtensions
{
    public class CosmopaladinCombatState(PlayerCombatState combatState)
    {
        private int _Determination;
        public event Action<int, int>? DeterminationChanged;

        public int Determination
        {
            get => _Determination;
            private set
            {
                if (_Determination == value)
                    return;
                int Determination = _Determination;
                _Determination = value;
                Action<int, int> determinationChanged = DeterminationChanged;
                if (determinationChanged == null)
                    return;
                determinationChanged(Determination, _Determination);
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