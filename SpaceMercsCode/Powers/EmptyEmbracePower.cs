using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Powers;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class EmptyEmbracePower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Void>(),
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card is Void)
        {
            foreach (var enemy in CombatState.HittableEnemies)
            {
                await CreatureCmd.Damage(choiceContext, enemy, new DamageVar(Amount, ValueProp.Unpowered), Owner, null);
            }
        }
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is EmptyEmbracePower && amount > 0 && Owner.Player != null)
        {
            foreach (var pile in Owner.Player.Piles)
            {
                if (pile.Type != PileType.Deck)
                {
                    RemoveEtherealFromVoidsInPile(pile);
                }
            }
        }
        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card is Void)
        {
            card.RemoveKeyword(CardKeyword.Ethereal);
        }
        return base.AfterCardGeneratedForCombat(card, creator);
    }

    private void RemoveEtherealFromVoidsInPile(CardPile pile)
    {
        foreach (var card in pile.Cards)
        {
            if (card is Void)
            {
                card.RemoveKeyword(CardKeyword.Ethereal);
            }
        }
    }
}