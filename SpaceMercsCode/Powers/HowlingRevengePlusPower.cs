using BaseLib.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class HowlingRevengePlusPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(BaseLibKeywords.Purge)
    ];

    private CardModel? _cardSource;

    public override bool ShouldDie(Creature creature) => creature != Owner;

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power == this && amount > 0)
        {
            _cardSource = cardSource;
        }
        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        Flash();
        await CreatureCmd.SetCurrentHp(Owner, (Decimal)(Owner.MaxHp * 0.75));
        await PowerCmd.Remove(this);
        if (_cardSource != null && _cardSource.DeckVersion != null)
        {
            await CardPileCmd.RemoveFromDeck(_cardSource.DeckVersion);
        }
    }
}