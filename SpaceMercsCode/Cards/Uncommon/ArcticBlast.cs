using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Keywords;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class ArcticBlast() : SpaceMercsCard(3,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "SPACEMERCS-DETERMINATION.title"), new LocString("static_hover_tips", "SPACEMERCS-DETERMINATION.description"))
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(20, ValueProp.Move),
        new PowerVar<SlowedPower>(2)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        SpaceMercsKeywords.Exert
    ];

    public override bool HasDeterminationAbility => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<SlowedPower>(choiceContext, enemy, DynamicVars[nameof(SlowedPower)].BaseValue,
                Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10);
        DynamicVars[nameof(SlowedPower)].UpgradeValueBy(2);
    }
}