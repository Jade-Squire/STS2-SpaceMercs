using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Enums;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class UnwaveringStarOath() : SpaceMercsCard(3,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override HashSet<CardTag> CanonicalTags =>
    [
        SpaceMercsTags.UnwaveringStar
    ];
    
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
            if (card is RememberedVow)
            {
                RemovedRememberedVow(card);
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
        
        CardCmd.TransformTo<AnswerTheCall>(this);
    }

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        if (card is BrokenOath)
        {
            CardCmd.TransformTo<UnwaveringStarBase>(this);
            newCard = card;
            return true;
        }
        return base.TryModifyCardBeingAddedToDeck(card, out newCard);
    }
}