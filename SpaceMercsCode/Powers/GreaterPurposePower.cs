using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Cards.Common;
using SpaceMercs.SpaceMercsCode.Cards.Rare;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class GreaterPurposePower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    private List<CardModel> _selectedCards = new();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> list = new();
            if (_selectedCards.Any())
            {
                foreach (var card in _selectedCards)
                {
                    list.Add(HoverTipFactory.FromCard(card));
                }
            }

            return list;
        }
    }

    public void AddSelectedCard(CardModel card)
    {
        _selectedCards.Add(card);
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side)
        {
            return;
        }

        foreach (var card in _selectedCards)
        {
            CardModel createdCard = card.CreateClone();
            BuffPermaCards(card);
            createdCard.AddKeyword(CardKeyword.Exhaust);
            await CardCmd.AutoPlay(new ThrowingPlayerChoiceContext(), createdCard, null);
            await CardPileCmd.RemoveFromCombat(createdCard);
        }
    }

    private void BuffPermaCards(CardModel card)
    {
        if (card is IPermaScalingCard)
        {
            if (card is RememberedVow or PhoenixRising or UnwaveringStarVow)
            {
                ((IPermaScalingCard)card).BuffCard();
            }

            if (card is GeneticAlgorithm algorithm)
            {
                algorithm.BuffFromPlay(algorithm.DynamicVars["Increase"].IntValue);
            }
        }
    }
}