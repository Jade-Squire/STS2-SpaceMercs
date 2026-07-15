using Godot;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Cards.Unique;
using SpaceMercs.SpaceMercsCode.Combat;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class BlindedPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }

    /*public override bool ShouldDraw(Player player, bool fromHandDraw)
    {
        if (player == Owner.Player)
        {
            return false;
        }
        return base.ShouldDraw(player, fromHandDraw);
    }

    public async Task<IEnumerable<CardModel>> DrawCard(PlayerChoiceContext choiceContext, Player player, Decimal count, bool fromHandDraw)
    {
        ICombatState combatState = player.Creature.CombatState;
        List<CardModel> result = new List<CardModel>();
        CardPile hand = PileType.Hand.GetPile(player);
        CardPile drawPile = PileType.Draw.GetPile(player);
        int drawsRequested = count > 0M ? (int) Math.Ceiling(count) : 0;
        if (drawsRequested == 0)
            return result;
        int num = Math.Max(0, CardPile.MaxCardsInHand - hand.Cards.Count);
        if (num == 0)
        {
            CardPileCmd.CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(player);
            return result;
        }
        for (int i = 0; i < drawsRequested && num > 0 && !CombatManager.Instance.IsOverOrEnding && CardPileCmd.CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(player); ++i)
        {
            await CardPileCmd.ShuffleIfNecessary(choiceContext, player);
            if (CardPileCmd.CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(player))
            {
                CardModel card = drawPile.Cards.FirstOrDefault<CardModel>();
                if (card != null && hand.Cards.Count < CardPile.MaxCardsInHand)
                {
                    UnknownBase unknown = CreateUnknown(card);
                    unknown.EnergyCost._base = card.EnergyCost.GetWithModifiers(CostModifiers.All);
                    unknown.ActualCard = card;
                    await CardPileCmd.AddGeneratedCardToCombat(unknown, PileType.Hand, Owner.Player);
                    card.RemoveFromCurrentPile();
                    await CardPileCmd.Add(card, SpacemercsCustomPile.Aether, CardPilePosition.Bottom, null, true);
                    result.Add(unknown);
                    CombatManager.Instance.History.CardDrawn(combatState, card, fromHandDraw);
                    await Hook.AfterCardDrawn(combatState, choiceContext, card, fromHandDraw);
                    card.InvokeDrawn();
                    NDebugAudioManager.Instance?.Play("card_deal.mp3", 0.25f, PitchVariance.Small);
                    num = Math.Max(0, CardPile.MaxCardsInHand - hand.Cards.Count);
                    card = null;
                }
                else
                    break;
            }
            else
                break;
        }

        return result;
    }*/

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner.Player)
        {
            return;
        }
        UnknownBase unknown = CreateUnknown(card);
        unknown.EnergyCost._base = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        unknown.ActualCard = card;
        await CardPileCmd.AddGeneratedCardToCombat(unknown, PileType.Hand, Owner.Player);
    }

    private UnknownBase CreateUnknown(CardModel card)
    {
        switch (card.TargetType)
        {
            case TargetType.AnyEnemy:
            case TargetType.AllEnemies:
            case TargetType.RandomEnemy:
                return card.CombatState.CreateCard<UnknownTargetEnemy>(card.Owner);
            case TargetType.Self:
            case TargetType.AllAllies:
            case TargetType.Osty:
                return card.CombatState.CreateCard<UnknownTargetSelf>(card.Owner);
            case TargetType.AnyPlayer:
                return card.CombatState.CreateCard<UnknownTargetAnyPlayer>(card.Owner);
            case TargetType.AnyAlly:
                return card.CombatState.CreateCard<UnknownTargetAlly>(card.Owner);
            default:
                GD.PrintErr($"Target type {card.TargetType} is not supported.");
                return card.CombatState.CreateCard<UnknownTargetEnemy>(card.Owner);
        }
    }
}