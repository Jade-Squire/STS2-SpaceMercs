using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class HammerStrike() : SpaceMercsCard(0,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CurePower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5M, ValueProp.Move),
        new PowerVar<CurePower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target!);
        int energyOnPlay = ResolveEnergyXValue();
        if (IsUpgraded)
        {
            energyOnPlay++;
        }
        if (energyOnPlay > 0 )
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue * energyOnPlay)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);
            await PowerCmd.Apply<CurePower>(choiceContext, Owner.Creature,
                DynamicVars[nameof(CurePower)].BaseValue * energyOnPlay, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(CurePower)].UpgradeValueBy(1);
    }
}