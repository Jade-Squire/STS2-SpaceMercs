using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class VengeancePowerBuff() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SuppressPower>(),
        HoverTipFactory.FromPower<HungerPower>()
    ];

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (result.UnblockedDamage > 0 && props == ValueProp.Move && target == Owner && dealer is { IsPlayer: false })
        {
            await PowerCmd.Apply<SuppressPower>(choiceContext, dealer, Amount, Owner, null);
            await PowerCmd.Apply<VengeancePowerDebuff>(choiceContext, dealer, 1, Owner, null);
        }
    }
}