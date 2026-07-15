using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Character;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class ChillingPast() : SpaceMercsCard(2,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        
    }

    protected override void OnUpgrade()
    {

    }
    
    public override Task BeforeCardRemoved(CardModel card)
    {
        if (Owner.Deck.Cards.Contains(this))
        {
            if (card is BrokenOath)
            {
                RemovedBrokenOath(card);
            }
        }

        return base.BeforeCardRemoved(card);
    }

    public void RemovedBrokenOath(CardModel cardRemoved)
    {
        // make sure theres no more vows
        foreach (var card in Owner.Deck.Cards)
        {
            if (card is BrokenOath && card != cardRemoved)
            {
                return;
            }
        }

        CardModel newCard = ModelDb.Card<NewPath>().ToMutable();
        newCard.Owner = Owner;
        if (IsUpgraded)
        {
            CardCmd.Upgrade(newCard, CardPreviewStyle.None);
        }

        CardCmd.Transform(this, newCard);
    }

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        if (card is RememberedVow)
        {
            CardModel endCard = ModelDb.Card<Indecisive>().ToMutable();
            endCard.Owner = Owner;
            if (IsUpgraded)
            {
                CardCmd.Upgrade(endCard, CardPreviewStyle.None);
            }
            
            CardCmd.Transform(this, endCard);
        }

        return base.TryModifyCardBeingAddedToDeck(card, out newCard);
    }
}