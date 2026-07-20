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
using SpaceMercs.SpaceMercsCode.Hooks;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class FrozenPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    public event Action<FrozenPower>? FreezeRemoved;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>()
    ];

    private MoveState _nextMove;
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await SpaceMercsHooks.AfterEnemyFrozen(CombatState, new ThrowingPlayerChoiceContext(), Owner, applier, cardSource);
        await StunCreature();
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            Action<FrozenPower>? freezeRemoved = FreezeRemoved;
            if (freezeRemoved != null)
            {
                freezeRemoved(this);
            }
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (cardSource != null && cardSource.Type == CardType.Attack && target == Owner)
        {
            if (Owner.IsStunned)
            {
                UnstunCreature();
                await CreatureCmd.Damage(choiceContext, Owner, new DamageVar(20, ValueProp.Unpowered), null, null);
                await PowerCmd.Remove(this);
                await PowerCmd.Apply<WeakPower>(choiceContext, Owner, 3, dealer, cardSource);
            }
        }
    }

    private async Task StunCreature()
    {
        _nextMove = Owner.Monster.NextMove;
        await CreatureCmd.Stun(Owner);
    }

    private void UnstunCreature()
    {
        Owner.Monster.SetMoveImmediate(_nextMove, true);
        Action<FrozenPower>? freezeRemoved = FreezeRemoved;
        if (freezeRemoved != null)
        {
            freezeRemoved(this);
        }
    }
}