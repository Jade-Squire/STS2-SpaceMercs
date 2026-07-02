using MegaCrit.Sts2.Core.Combat;
using SpaceMercs.SpaceMercsCode.CombatState;

namespace SpaceMercs.SpaceMercsCode.Extensions;

public static class CombatStateTrackerExtension
{
    private static void OnDeterminationChanged(this CombatStateTracker tracker, int _, int __)
    {
        tracker.NotifyCombatStateChanged("onPlayerCombatStateValueChanged");
    }

    public static void SubscribeDetermination(this CombatStateTracker tracker,
        PlayerCombatStateExtensions.CosmopaladinCombatState combatState)
    {
        combatState.DeterminationChanged += tracker.OnDeterminationChanged;
    }

    public static void UnsubscribeDetermination(this CombatStateTracker tracker,
        PlayerCombatStateExtensions.CosmopaladinCombatState combatState)
    {
        combatState.DeterminationChanged -= tracker.OnDeterminationChanged;
    }
}