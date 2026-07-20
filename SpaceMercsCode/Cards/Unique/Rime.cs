using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Combat;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Unique;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class Rime() : SpaceMercsCard(0,
    CardType.Status, CardRarity.None,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SlowedPower>(),
        HoverTipFactory.FromPower<FrozenPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        IEnumerable<CardModel> slowCards = ModelDb.CardPool<CosmopaladinCardPool>().AllCards.Where(card => card.Tags.Contains(SpaceMercsTags.Slows));
        CardModel? generatedCard = CardFactory.GetDistinctForCombat(Owner, slowCards, 1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (generatedCard != null)
        {
            if (IsUpgraded)
            {
                generatedCard.EnergyCost.SetThisCombat(0);
            }
            else
            {
                generatedCard.EnergyCost.SetUntilPlayed(0);
            }
            await CardPileCmd.AddGeneratedCardToCombat(generatedCard, PileType.Hand, Owner);
        }
        
        List<CardModel> cardsToRemove = new();
        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card is Reason)
            {
                cardsToRemove.Add(card);
            }
        }

        foreach (var card in cardsToRemove)
        {
            await CardPileCmd.RemoveFromCombat(card);
        }
        
        ReturnCardsToHand();
        
        await CardPileCmd.RemoveFromCombat(this);
    }

    protected override void OnUpgrade()
    {

    }
    
    private void ReturnCardsToHand()
    {
        List<CardModel> cardsToMove = new();
        foreach (var card in SpacemercsCustomPile.Aether.GetPile(Owner).Cards)
        {
            cardsToMove.Add(card);
        }
        
        foreach (var card in cardsToMove)
        {
            CardPileCmd.Add(card, PileType.Hand);
        }
    }
}