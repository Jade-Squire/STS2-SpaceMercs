using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Hooks;
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

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is HungerPower && amount > 0 && Owner.Player != null && power == this)
        {
            await PlayerCmd.GainEnergy(amount, Owner.Player);
            await CardPileCmd.AddToCombatAndPreview<Void>(Owner, PileType.Draw, (int)amount, Owner.Player, CardPilePosition.Random);
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (Owner.IsAlive && Owner.Player != null)
        {
            Flash();
            await PlayerCmd.GainEnergy(Amount, Owner.Player);
            AbstractModel? preventer;
            if (!SpaceMercsHooks.ShouldLoseHungerAtTurnStart(combatState, choiceContext, out preventer))
            {
                if (preventer != null && preventer is IShouldLoseHunger)
                    ((IShouldLoseHunger)preventer).AfterHungerLossPrevented();
            }
            else
                await PowerCmd.Remove(this);
        }
    }
}