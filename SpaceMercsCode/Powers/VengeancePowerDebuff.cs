using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class VengeancePowerDebuff() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<HungerPower>()
    ];

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if(wasRemovalPrevented)
            return;
        foreach (var player in CombatState.Players)
        {
            if (player.Creature.HasPower<VengeancePowerBuff>())
            {
                await PowerCmd.Apply<HungerPower>(choiceContext, player.Creature, 10 * player.Creature.GetPowerAmount<VengeancePowerBuff>(), player.Creature, null);
            }
        }
    }
}