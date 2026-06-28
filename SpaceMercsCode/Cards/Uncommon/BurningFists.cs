using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.GameInfo.Objects;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class BurningFists() : SpaceMercsCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<CurePower>(),
        HoverTipFactory.FromPower<ScorchPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Ethereal
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_slash")
            .Targeting(play.Target)
            .Execute(choiceContext);

        if (Owner.Creature.HasPower<CurePower>())
        {
            await PowerCmd.Apply<ScorchPower>(choiceContext, play.Target, Owner.Creature.GetPowerAmount<CurePower>(), Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}