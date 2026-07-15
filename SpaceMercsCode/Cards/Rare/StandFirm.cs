using System.Xml.Xsl;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Combat;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class StandFirm() : SpaceMercsCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<FrostArmorPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FrostArmorPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<FrostArmorPower>(choiceContext, Owner.Creature,
            DynamicVars[nameof(FrostArmorPower)].BaseValue, Owner.Creature, this);
        
        List<IPermaScalingCard> cardsToBuff = new();
        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card is IPermaScalingCard permaScalingCard)
            {
                cardsToBuff.Add(permaScalingCard);
            }
            else if (card is GeneticAlgorithm geneticAlgorithm)
            {
                geneticAlgorithm.BuffFromPlay(geneticAlgorithm._dynamicVars[GeneticAlgorithm._increaseKey].IntValue);
                if (geneticAlgorithm.DeckVersion is GeneticAlgorithm deckVersion)
                {
                    deckVersion.BuffFromPlay(deckVersion._dynamicVars[GeneticAlgorithm._increaseKey].IntValue);
                }
            }
        }

        foreach (var card in PileType.Draw.GetPile(Owner).Cards)
        {
            if (card is IPermaScalingCard permaScalingCard)
            {
                cardsToBuff.Add(permaScalingCard);
            }
            else if (card is GeneticAlgorithm geneticAlgorithm)
            {
                geneticAlgorithm.BuffFromPlay(geneticAlgorithm._dynamicVars[GeneticAlgorithm._increaseKey].IntValue);
                if (geneticAlgorithm.DeckVersion is GeneticAlgorithm deckVersion)
                {
                    deckVersion.BuffFromPlay(deckVersion._dynamicVars[GeneticAlgorithm._increaseKey].IntValue);
                }
            }
        }

        foreach (var card in PileType.Discard.GetPile(Owner).Cards)
        {
            if (card is IPermaScalingCard permaScalingCard)
            {
                cardsToBuff.Add(permaScalingCard);
            }
            else if (card is GeneticAlgorithm geneticAlgorithm)
            {
                geneticAlgorithm.BuffFromPlay(geneticAlgorithm._dynamicVars[GeneticAlgorithm._increaseKey].IntValue);
                if (geneticAlgorithm.DeckVersion is GeneticAlgorithm deckVersion)
                {
                    deckVersion.BuffFromPlay(deckVersion._dynamicVars[GeneticAlgorithm._increaseKey].IntValue);
                }
            }
        }

        foreach (var card in PileType.Exhaust.GetPile(Owner).Cards)
        {
            if (card is IPermaScalingCard permaScalingCard)
            {
                cardsToBuff.Add(permaScalingCard);
            }
            else if (card is GeneticAlgorithm geneticAlgorithm)
            {
                geneticAlgorithm.BuffFromPlay(geneticAlgorithm._dynamicVars[GeneticAlgorithm._increaseKey].IntValue);
                if (geneticAlgorithm.DeckVersion is GeneticAlgorithm deckVersion)
                {
                    deckVersion.BuffFromPlay(deckVersion._dynamicVars[GeneticAlgorithm._increaseKey].IntValue);
                }
            }
        }

        foreach (var card in PileType.Play.GetPile(Owner).Cards)
        {
            if (card is IPermaScalingCard permaScalingCard)
            {
                cardsToBuff.Add(permaScalingCard);
            }
            else if (card is GeneticAlgorithm geneticAlgorithm)
            {
                geneticAlgorithm.BuffFromPlay(geneticAlgorithm._dynamicVars[GeneticAlgorithm._increaseKey].IntValue);
                if (geneticAlgorithm.DeckVersion is GeneticAlgorithm deckVersion)
                {
                    deckVersion.BuffFromPlay(deckVersion._dynamicVars[GeneticAlgorithm._increaseKey].IntValue);
                }
            }
        }

        foreach (var card in SpacemercsCustomPile.Aether.GetPile(Owner).Cards)
        {
            if (card is IPermaScalingCard permaScalingCard)
            {
                cardsToBuff.Add(permaScalingCard);
            }
            else if (card is GeneticAlgorithm geneticAlgorithm)
            {
                geneticAlgorithm.BuffFromPlay(geneticAlgorithm._dynamicVars[GeneticAlgorithm._increaseKey].IntValue);
                if (geneticAlgorithm.DeckVersion is GeneticAlgorithm deckVersion)
                {
                    deckVersion.BuffFromPlay(deckVersion._dynamicVars[GeneticAlgorithm._increaseKey].IntValue);
                }
            }
        }

        foreach (var card in cardsToBuff)
        {
            card.BuffCard();
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
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

    public void RemovedRememberedVow(CardModel cardRemoved)
    {
        // make sure theres no more vows
        foreach (var card in Owner.Deck.Cards)
        {
            if (card is RememberedVow && card != cardRemoved)
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
        if (card is BrokenOath)
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