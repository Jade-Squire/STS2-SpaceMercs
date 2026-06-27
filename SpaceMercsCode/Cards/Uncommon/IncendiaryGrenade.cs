using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class IncendiaryGrenade() : SpaceMercsCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{ 
    protected override HashSet<CardTag> CanonicalTags =>
    [
        SpaceMercsTags.Grenade
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ScorchPower>(),
        HoverTipFactory.FromPower<CurePower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ScorchPower>(3),
        new PowerVar<CurePower>(3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target!);
        await PowerCmd.Apply<ScorchPower>(choiceContext, play.Target, DynamicVars[nameof(ScorchPower)].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<CurePower>(choiceContext, Owner.Creature, DynamicVars[nameof(CurePower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(ScorchPower)].UpgradeValueBy(2);
    }
}