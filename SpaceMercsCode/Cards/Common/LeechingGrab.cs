using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class LeechingGrab() : SpaceMercsCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(2, ValueProp.Move),
        new BlockVar(1, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int totalStacks = 0;
        List<PowerModel> powersToRemove = new();
        foreach (var power in play.Target.Powers)
        {
            if (power.Type == PowerType.Debuff)
            {
                totalStacks += power.Amount;
                powersToRemove.Add(power);
            }
        }

        foreach (var power in powersToRemove)
        {
            PowerCmd.Remove(power);
        }

        if (totalStacks > 0)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_blunt")
                .WithHitCount(totalStacks)
                .Execute(choiceContext);
            for (int i = 0; i < totalStacks; i++)
            {
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1);
        DynamicVars.Damage.UpgradeValueBy(1);
    }
}