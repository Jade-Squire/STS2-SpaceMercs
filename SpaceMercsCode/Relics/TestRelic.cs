using MegaCrit.Sts2.Core.Entities.Creatures;
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

public class TestRelic() : SpaceMercsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    private int _hungerGained = 0;
    private bool _gainedExtraReward = false;

    [SavedProperty]
    public bool GainedExtraReward
    {
        get => _gainedExtraReward;
        private set
        {
            AssertMutable();
            _gainedExtraReward = value;
        }
    }

    public override ActMap ModifyGeneratedMap(IRunState runState, ActMap map, int actIndex)
    {
        if (actIndex == 0)
        {
            map.StartingMapPoint.PointType = MapPointType.Shop;
        }        
        return map;
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        if (!GainedExtraReward && _hungerGained >= 10)
        {
            CardModel card = ModelDb.Card<Starvation>().ToMutable();
            Owner.RunState.AddCard(card, Owner);
            room.AddExtraReward(Owner, new SpecialCardReward(card, Owner));
            GainedExtraReward = true;
        }

        _hungerGained = 0;
        return base.AfterCombatVictory(room);
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner == Owner.Creature && power is HungerPower && amount > 0)
        {
            _hungerGained += (int)amount;
        }
        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }
}