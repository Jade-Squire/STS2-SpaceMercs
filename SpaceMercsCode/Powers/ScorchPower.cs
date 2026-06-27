using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;


public class ScorchPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    private bool _hasGainedScorch = false;

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != Owner.Side)
        {
            return base.AfterSideTurnStart(side, participants, combatState);
        }
        if (!_hasGainedScorch)
        {
            PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -Math.Max(Amount / 2, 1), null, null);
        }
        _hasGainedScorch = false;
        return base.AfterSideTurnStart(side, participants, combatState);
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        //TODO: try to add a delay to show you reached 10 stacks before removing and damaging
        if (power is ScorchPower && power.Owner == Owner)
        {
            if (amount > 0)
            {
                _hasGainedScorch = true;
            }
            if (power.Amount >= 10)
            {
                List<Creature> creatures = new List<Creature>();
                foreach (var currCreature in CombatState.Enemies)
                {
                    if (currCreature.IsAlive && currCreature != power.Owner)
                    {
                        creatures.Add(currCreature);
                    }
                }

                await Cmd.CustomScaledWait(0.2f, 0.4f);
                
                foreach (var currCreature in creatures)
                {
                    await CreatureCmd.Damage(choiceContext, currCreature, 15M, ValueProp.Unpowered, null, null);
                }
                await CreatureCmd.Damage(choiceContext, power.Owner, 30M, ValueProp.Unpowered, null, null);
                await PowerCmd.ModifyAmount(choiceContext, power, -10, applier, cardSource);
            }
        }
    }   
}