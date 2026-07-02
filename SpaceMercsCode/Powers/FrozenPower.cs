using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class FrozenPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>()
    ];

    private MoveState _nextMove;
    
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        StunCreature();
        return base.AfterApplied(applier, cardSource);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if(side == Owner.Side)
            PowerCmd.Remove(this);
        return base.AfterSideTurnEnd(choiceContext, side, participants);
    }

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (cardSource != null && cardSource.Type == CardType.Attack)
        {
            if (Owner.IsStunned)
            {
                UnstunCreature();
                CreatureCmd.Damage(choiceContext, Owner, new DamageVar(20, ValueProp.Unpowered), null, null);
                PowerCmd.Remove(this);
                PowerCmd.Apply<WeakPower>(choiceContext, Owner, 3, dealer, cardSource);
            }
        }
        return base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);
    }

    private void StunCreature()
    {
        _nextMove = Owner.Monster.NextMove;
        CreatureCmd.Stun(Owner);
    }

    private void UnstunCreature()
    {
        Owner.Monster.SetMoveImmediate(_nextMove, true);
    }
}