using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Unique;

public abstract class UnknownBase(int cost, CardType type, CardRarity rarity, TargetType targetType) : SpaceMercsCard(cost, type, rarity, targetType)
{
    public CardModel? ActualCard;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (ActualCard != null)
        {
            await CardPileCmd.RemoveFromCombat(this);
            choiceContext.PopModel(this);
            await ActualCard.OnPlayWrapper(choiceContext, play.Target, true, play.Resources);
            choiceContext.PushModel(this);
        }
    }

    protected override void OnUpgrade()
    {

    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card == this && card.Pile.Type != PileType.Hand && card.Pile.Type != PileType.Play)
        {
            await CardPileCmd.Add(ActualCard, card.Pile.Type, CardPilePosition.Bottom, null, true);
            await CardPileCmd.RemoveFromCombat(this, true);
        }
    }
}