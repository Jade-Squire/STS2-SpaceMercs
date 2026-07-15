using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Hooks;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class SlowedPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FrozenPower>()
    ];

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side && Owner.IsAlive)
        {
            await PowerCmd.Apply<SlowedPower>(new ThrowingPlayerChoiceContext(), Owner, Amount, null, null);
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (amount > 0 && power is SlowedPower && Amount >= 20)
        {
            await PowerCmd.Remove<SlowedPower>(Owner);
            await SpaceMercsHooks.BeforeEnemyFrozen(CombatState, choiceContext, Owner, applier, cardSource);
            await PowerCmd.Apply<FrozenPower>(choiceContext, Owner, 1, applier, cardSource);
            await SpaceMercsHooks.AfterEnemyFrozen(CombatState, choiceContext, Owner, applier, cardSource);
        }
    }
}