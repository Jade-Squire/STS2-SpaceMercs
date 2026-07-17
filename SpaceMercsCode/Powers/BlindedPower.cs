using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards.Unique;

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
                //GD.PrintErr($"Target type {card.TargetType} is not supported.");
                return card.CombatState.CreateCard<UnknownTargetSelf>(card.Owner);
        }
    }
}