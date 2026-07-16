using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Combat;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class NewPath() : SpaceMercsCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        List<CardModel> cardsInCombat = PileType.Draw.GetPile(Owner).Cards.ToList();
        cardsInCombat = cardsInCombat.Concat(PileType.Hand.GetPile(Owner).Cards).ToList();
        cardsInCombat = cardsInCombat.Concat(PileType.Discard.GetPile(Owner).Cards).ToList();
        cardsInCombat = cardsInCombat.Concat(PileType.Exhaust.GetPile(Owner).Cards).ToList();
        cardsInCombat = cardsInCombat.Concat(SpacemercsCustomPile.Aether.GetPile(Owner).Cards).ToList();

        IEnumerable<CardModel> cardsToRemove = await CardSelectCmd.FromSimpleGrid(choiceContext, cardsInCombat, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 0, 3));

        foreach (CardModel card in cardsToRemove)
        {
            await CardPileCmd.RemoveFromCombat(card);
            if (card.DeckVersion != null)
            {
                await CardPileCmd.RemoveFromDeck(card.DeckVersion);
            }
        }
        
        if(cardsToRemove.Count() > 0)
            await AddCardOfRarityToCombatAndDeck(choiceContext, CardRarity.Common);
        if(cardsToRemove.Count() > 1)
            await AddCardOfRarityToCombatAndDeck(choiceContext, CardRarity.Uncommon);
        if(cardsToRemove.Count() > 2)
            await AddCardOfRarityToCombatAndDeck(choiceContext, CardRarity.Rare);

        if (IsUpgraded)
        {
            foreach (var enemy in CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<FrozenPower>(choiceContext, enemy, 1, Owner.Creature, this);
            }
        }

        PlayerCmd.EndTurn(Owner, false);

        await CardPileCmd.RemoveFromCombat(this);
        if (DeckVersion != null)
        {
            await CardPileCmd.RemoveFromDeck(DeckVersion);
        }
    }

    protected override void OnUpgrade()
    {

    }
    
    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        if (card is RememberedVow)
        {
            CardModel endCard = ModelDb.Card<StandFirm>().ToMutable();
            endCard.Owner = Owner;
            if (IsUpgraded)
            {
                CardCmd.Upgrade(endCard, CardPreviewStyle.None);
            }

            CardCmd.Transform(this, endCard);
        }
        else if (card is BrokenOath)
        {
            CardModel endCard = ModelDb.Card<ChillingPast>().ToMutable();
            endCard.Owner = Owner;
            if (IsUpgraded)
            {
                CardCmd.Upgrade(endCard, CardPreviewStyle.None);
            }

            CardCmd.Transform(this, endCard);
        }
        return base.TryModifyCardBeingAddedToDeck(card, out newCard);
    }

    private async Task AddCardOfRarityToCombatAndDeck(PlayerChoiceContext choiceContext, CardRarity rarity)
    {
        IEnumerable<CardModel> pool = ModelDb.CardPool<CosmopaladinCardPool>().AllCards
            .Where(card => card.Rarity == rarity && card is not UnwaveringStarBase or Indecisive);

        List<CardModel> cardsToChooseFrom = new();

        foreach (var card in pool)
        {
            cardsToChooseFrom.Add(RunState.CreateCard(card, Owner));
        }
        
        IEnumerable<CardModel> cardSelected = await CardSelectCmd.FromSimpleGrid(choiceContext, cardsToChooseFrom, Owner, new CardSelectorPrefs(new LocString("cards", Id.Entry + ".select" + rarity),1));
        
        CardModel? cardToAdd = cardSelected.FirstOrDefault();
        
        if (cardToAdd != null)
        {
            await CardPileCmd.Add(cardToAdd, PileType.Deck);
            CardModel combatCard = CombatState.CloneCard(cardToAdd);
            combatCard.DeckVersion = cardToAdd;
            await CardPileCmd.Add(combatCard, PileType.Hand);
        }
    }
}