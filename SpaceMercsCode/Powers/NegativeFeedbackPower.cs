using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class NegativeFeedbackPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<JoltPower>(),
        HoverTipFactory.FromPower<AmpPower>()
    ];

    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (amount > 0 && target == Owner && target.HasPower<AmpPower>())
        {
            foreach (var enemy in CombatState.HittableEnemies)
            {
                Flash();
                PowerCmd.Apply<JoltPower>(new ThrowingPlayerChoiceContext(), enemy, Amount, Owner, null);
            }
        }
        return base.ModifyHpLostAfterOstyLate(target, amount, props, dealer, cardSource);
    }
}