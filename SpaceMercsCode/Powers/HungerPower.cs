using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Powers;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class HungerPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Void>()
    ];

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is HungerPower && amount > 0 && Owner.Player != null && power == this)
        {
            PlayerCmd.GainEnergy(amount, Owner.Player);
            CardPileCmd.AddToCombatAndPreview<Void>(Owner, PileType.Draw, (int)amount, Owner.Player, CardPilePosition.Random);
        }
        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == Owner.Side && Owner.IsAlive && Owner.Player != null)
        {
            Flash();
            PlayerCmd.GainEnergy(Amount, Owner.Player);
            PowerCmd.Remove(this);
        }
        return base.AfterSideTurnStart(side, participants, combatState);
    }
}