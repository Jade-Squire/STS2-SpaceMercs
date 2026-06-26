using Godot;
using Godot.Collections;
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

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        //TODO: try to add a delay to show you reached 10 stacks before removing and damaging
        if (power is ScorchPower)
        {
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

                foreach (var currCreature in creatures)
                {
                    CreatureCmd.Damage(choiceContext, currCreature, 15M, ValueProp.Unpowered, null, null);
                }
                CreatureCmd.Damage(choiceContext, power.Owner, 30M, ValueProp.Unpowered, null, null);
                PowerCmd.ModifyAmount(choiceContext, power, -10, applier, cardSource);
            }
        }

        return Task.CompletedTask;
    }   
}