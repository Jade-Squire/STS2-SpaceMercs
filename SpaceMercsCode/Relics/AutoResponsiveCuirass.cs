using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using SpaceMercs.SpaceMercsCode.Cards.Rare;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Relics;

public class AutoResponsiveCuirass() : SpaceMercsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    private int _hungerGained = 0;
    
    private bool _debuffed = false;
    private bool _buffed = false;
    private bool _hasGainedEnergy = false;

    [SavedProperty]
    public bool GainedExtraReward
    {
        get;
        private set
        {
            AssertMutable();
            field = value;
        }
    } = false;

    public override Task AfterCombatVictory(CombatRoom room)
    {
        if (!GainedExtraReward && _hungerGained >= 10)
        {
            CardModel card = ModelDb.Card<Starvation>().ToMutable();
            Owner.RunState.AddCard(card, Owner);
            room.AddExtraReward(Owner, new SpecialCardReward(card, Owner));
            GainedExtraReward = true;
        }

        _buffed = false;
        _debuffed = false;
        _hasGainedEnergy = false;
        _hungerGained = 0;
        return base.AfterCombatVictory(room);
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner == Owner.Creature && power is HungerPower && amount > 0)
        {
            _hungerGained += (int)amount;
        }

        if (applier == Owner.Creature)
        {
            if (!_buffed && power.Type == PowerType.Buff)
            {
                _buffed = true;
            }

            if (!_debuffed && power.Type == PowerType.Debuff)
            {
                _debuffed = true;
            }

            if (_buffed && _debuffed && !_hasGainedEnergy)
            {
                _hasGainedEnergy = true;
                await PlayerCmd.GainEnergy(1, Owner);
                Flash();
            }
        }
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        _debuffed = false;
        _buffed = false;
        return base.AfterSideTurnStart(side, participants, combatState);
    }
}