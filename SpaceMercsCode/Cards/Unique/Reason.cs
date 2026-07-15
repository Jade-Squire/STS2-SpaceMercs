using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Combat;
using SpaceMercs.SpaceMercsCode.Enums;

namespace SpaceMercs.SpaceMercsCode.Cards.Unique;

public class Reason() : SpaceMercsCard(0,
    CardType.Status, CardRarity.Token,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardModel? generatedCard = CardFactory.GetDistinctForCombat(Owner, ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint), 1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault<CardModel>();
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
            if (card is Rime)
            {
                cardsToRemove.Add(card);
            }
        }

        foreach (var card in cardsToRemove)
        {
            await CardPileCmd.RemoveFromCombat(card);
        }
        
        ReturnCardsToHand();
    }

    public override Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        List<CardModel> cardsToRemove = new();
        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card is Rime or Reason)
            {
                cardsToRemove.Add(card);
            }
        }

        foreach (var card in cardsToRemove)
        {
            CardPileCmd.RemoveFromCombat(card);
        }
        
        ReturnCardsToHand();
        return base.BeforeSideTurnEnd(choiceContext, side, participants);
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

    protected override void OnUpgrade()
    {

    }
}