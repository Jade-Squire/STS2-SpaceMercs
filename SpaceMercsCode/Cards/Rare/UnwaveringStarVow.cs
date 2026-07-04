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
public class UnwaveringStarVow() : SpaceMercsCard(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
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
            if (card is BrokenOath)
            {
                RemovedBrokenOath(card);
            }
        }
        return base.BeforeCardRemoved(card);
    }

    private void RemovedBrokenOath(CardModel cardRemoved)
    {
        // make sure theres no more vows
        foreach (var card in Owner.Deck.Cards)
        {
            if (card is BrokenOath && card != cardRemoved)
            {
                return;
            }
        }
        
        CardCmd.TransformTo<AnswerTheCall>(this);
    }

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        if (card is RememberedVow)
        {
            CardCmd.TransformTo<UnwaveringStarBase>(this);
        }
        return base.TryModifyCardBeingAddedToDeck(card, out newCard);
    }
}