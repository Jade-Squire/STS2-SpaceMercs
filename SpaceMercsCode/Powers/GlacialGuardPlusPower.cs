using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class GlacialGuardPlusPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SlowedPower>()
    ];

    public override async Task AfterBlockBroken(Creature creature)
    {
        if (creature == Owner)
        {
            foreach (var enemy in CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<SlowedPower>(new ThrowingPlayerChoiceContext(), enemy, Amount, Owner, null);
            }
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if(side == Owner.Side)
            await PowerCmd.Remove(this);
    }
}