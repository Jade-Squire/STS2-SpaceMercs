using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class CurePower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    private bool _canLoseStacks = true;
    private bool _overrideCanLoseStacks = false;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.IsDead)
            return;
        Flash();
        await CreatureCmd.Heal(Owner, Amount);
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner) || Owner.IsDead)
        {
            return base.AfterSideTurnStart(side, participants, combatState);
        }
        _canLoseStacks = true;
        _overrideCanLoseStacks = false;
        return base.AfterSideTurnStart(side, participants, combatState);
    }

    public void CanLoseStacksThisTurn(bool canLoseStacks)
    {
        _canLoseStacks = canLoseStacks;
        _overrideCanLoseStacks = true;
    }

    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (target == Owner && amount > 0 && _canLoseStacks)
        {
            if (Amount == 1)
            {
                PowerCmd.Remove(this);
            }
            else
            {
                PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -(Amount / 2), Owner, null);
            }
        }

        return base.ModifyHpLostAfterOsty(target, amount, props, dealer, cardSource);
    }
}