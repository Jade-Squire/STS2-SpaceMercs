using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class AshesToAshesPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner && cardPlay.Card.Tags.Contains(SpaceMercsTags.Grenade) &&
            !cardPlay.Card.Keywords.Contains(CardKeyword.Exhaust))
        {
            CardModel card = cardPlay.Card.CreateClone();
            card.AddKeyword(CardKeyword.Exhaust);
            card.EnergyCost.SetThisCombat(0);
            for (int i = 0; i < Amount; i++)
            {
                await CardPileCmd.AddGeneratedCardToCombat(card.CreateClone(), PileType.Hand, cardPlay.Card.Owner);
            }
        }
    }
}