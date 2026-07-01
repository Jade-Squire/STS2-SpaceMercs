using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class BurningTreads() : SpaceMercsCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    public override CardMultiplayerConstraint MultiplayerConstraint
    {
        get => CardMultiplayerConstraint.MultiplayerOnly;
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ScorchPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6, ValueProp.Move),
        new PowerVar<ScorchPower>(1),
        new PowerVar<BurningTreadsPower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Targeting(play.Target)
            .Execute(choiceContext);
        await PowerCmd.Apply<ScorchPower>(choiceContext, play.Target, DynamicVars[nameof(ScorchPower)].BaseValue,
            Owner.Creature, this);
        await PowerCmd.Apply<BurningTreadsPower>(choiceContext, play.Target,
            DynamicVars[nameof(BurningTreadsPower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}