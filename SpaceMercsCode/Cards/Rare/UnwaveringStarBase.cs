using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Enums;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

public class UnwaveringStarBase() : SpaceMercsCard(0,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Unplayable
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        
    }

    public override Task BeforeCardRemoved(CardModel card)
    {
        if (Owner.Deck.Cards.Contains(this))
        {
            if (card is RememberedVow)
            {
                RemovedRememberedVow(card);
            }
            else if (card is BrokenOath)
            {
                RemovedBrokenOath(card);
            }
        }
        return base.BeforeCardRemoved(card);
    }

    private void RemovedRememberedVow(CardModel cardRemoved)
    {
        // make sure theres no more vows
        foreach (var card in Owner.Deck.Cards)
        {
            if (card is RememberedVow && card != cardRemoved)
            {
                return;
            }
        }
        
        foreach (var card in Owner.Deck.Cards)
        {
            if (card is BrokenOath && card != cardRemoved)
            {
                CardModel newCard = ModelDb.Card<UnwaveringStarVow>().ToMutable();
                newCard.Owner = Owner;
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(newCard, CardPreviewStyle.None);
                }

                CardCmd.Transform(this, newCard);
                return;
            }
        }
        
        CardCmd.TransformTo<AnswerTheCall>(this);
    }

    private void RemovedBrokenOath(CardModel cardRemoved)
    {
        foreach (var card in Owner.Deck.Cards)
        {
            if (card is BrokenOath && card != cardRemoved)
            {
                return;
            }
        }

        foreach (var card in Owner.Deck.Cards)
        {
            if (card is RememberedVow && card != cardRemoved)
            {
                CardModel newCard = ModelDb.Card<UnwaveringStarOath>().ToMutable();
                newCard.Owner = Owner;
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(newCard, CardPreviewStyle.None);
                }

                CardCmd.Transform(this, newCard);
                return;
            }
        }
        
        CardCmd.TransformTo<AnswerTheCall>(this);
    }
}