using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Hooks;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class QuickAndRelentlessPower() : SpaceMercsPower, IEnemyJolted
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<JoltPower>(),
        HoverTipFactory.FromPower<SlowedPower>(),
        HoverTipFactory.FromPower<FrozenPower>()
    ];

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Decrement(this);
        }
    }

    public async void AfterEnemyJolted(PlayerChoiceContext choiceContext, Creature joltedCreature, Creature? dealer,
        CardModel? cardSource, int joltUsed)
    {
        if (joltedCreature.IsHittable)
        {
            await PowerCmd.Apply<SlowedPower>(choiceContext, joltedCreature, joltUsed, Owner, null);
        }
    }
}