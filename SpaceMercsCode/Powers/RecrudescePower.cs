using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Hooks;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class RecrudescePower() : SpaceMercsPower, IEnemyIgnited
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ScorchPower>(),
        HoverTipFactory.FromPower<JoltPower>()
    ];

    public async void AfterEnemyIgnited(PlayerChoiceContext choiceContext, Creature ignitedCreature, Creature? applier,
        CardModel? cardSource)
    {
        foreach (var creature in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<ScorchPower>(choiceContext, creature, Amount, Owner, null);
            await PowerCmd.Apply<JoltPower>(choiceContext, creature, Amount, Owner, null);
        }
    }
}