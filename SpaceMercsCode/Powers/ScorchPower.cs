using BaseLib.Extensions;
using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Hooks;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;


public class ScorchPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    private bool _hasGainedScorch = false;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != Owner.Side)
        {
            return;
        }
        if (!_hasGainedScorch)
        {
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -Math.Max(Amount / 2, 1), null, null);
        }
        _hasGainedScorch = false;
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is ScorchPower && power.Owner == Owner)
        {
            if (amount > 0)
            {
                _hasGainedScorch = true;
            }
            if (power.Amount >= 10)
            {
                await SpaceMercsHooks.BeforeEnemyIgnited(CombatState, choiceContext, power.Owner, applier,
                    cardSource);
                List<Creature> creatures = new List<Creature>();
                foreach (var currCreature in CombatState.Enemies)
                {
                    if (currCreature.IsAlive && currCreature != power.Owner)
                    {
                        creatures.Add(currCreature);
                    }
                }

                decimal damageToDeal = 15M;
                
                foreach (var player in CombatState.Players)
                {
                    if (player.HasPower<ForgeMasterPower>())
                    {
                        damageToDeal = 30M;
                    }
                }

                await Cmd.CustomScaledWait(0.2f, 0.4f);
                
                foreach (var currCreature in creatures)
                {
                    if (applier != null && applier.HasPower<ForgeMasterPower>())
                    {
                        await PowerCmd.Apply<ScorchPower>(choiceContext, currCreature, applier.GetPowerAmount<ForgeMasterPower>(), applier, null);
                    }
                    await CreatureCmd.Damage(choiceContext, currCreature, damageToDeal, ValueProp.Unpowered, null, null);
                }
                var combatState = CombatState;
                await CreatureCmd.Damage(choiceContext, power.Owner, 30M, ValueProp.Unpowered, null, null);
                await PowerCmd.ModifyAmount(choiceContext, power, -10, applier, cardSource);
                await SpaceMercsHooks.AfterEnemyIgnited(combatState, choiceContext, power.Owner, applier,
                    cardSource);
            }
        }
    }   
}