using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Keywords;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

public class Frostburn() : SpaceMercsCard(3,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        SpaceMercsKeywords.Exert
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ScorchPower>(),
        HoverTipFactory.FromPower<SlowedPower>(),
        HoverTipFactory.FromPower<FrostArmorPower>()
    ];

    protected override int CanonicalDeterminationCost => -1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ScorchPower>(6),
        new PowerVar<SlowedPower>(6),
        new DamageVar(6, ValueProp.Move),
        new PowerVar<FrostArmorPower>(1)
    ];

    public override bool HasDeterminationAbility => true;

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
        await PowerCmd.Apply<SlowedPower>(choiceContext, play.Target, DynamicVars[nameof(SlowedPower)].BaseValue,
            Owner.Creature, this);
        await PowerCmd.Apply<FrostArmorPower>(choiceContext, Owner.Creature,
            DynamicVars[nameof(FrostArmorPower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(ScorchPower)].UpgradeValueBy(2);
        DynamicVars[nameof(SlowedPower)].UpgradeValueBy(2);
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}