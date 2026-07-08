using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class DirePredicament() : SpaceMercsCard(4,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HealVar(30),
        new BlockVar(30, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        List<PowerModel> powersToRemove = new();
        foreach (var power in Owner.Creature.Powers)
        {
            if (power.Type == PowerType.Debuff)
            {
                powersToRemove.Add(power);
            }
        }

        foreach (var power in powersToRemove)
        {
            PowerCmd.Remove(power);
        }
        
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        List<CardModel> cardsToExhaust = new();
        
        if (!IsUpgraded)
        {
            foreach (var card in PileType.Hand.GetPile(Owner).Cards)
            {
                cardsToExhaust.Add(card);
            }
        }

        foreach (var card in PileType.Discard.GetPile(Owner).Cards)
        {
            cardsToExhaust.Add(card);
        }

        foreach (var card in cardsToExhaust)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
    }

    protected override void OnUpgrade()
    {

    }
}