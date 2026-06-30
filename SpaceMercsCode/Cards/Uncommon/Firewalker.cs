using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class Firewalker() : SpaceMercsCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ScorchPower>(),
        HoverTipFactory.FromPower<ArtifactPower>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Ethereal
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ScorchPower>(2),
        new PowerVar<ArtifactPower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<ScorchPower>(choiceContext, enemy, DynamicVars[nameof(ScorchPower)].BaseValue,
                Owner.Creature, this);
        }

        await PowerCmd.Apply<ArtifactPower>(choiceContext, Owner.Creature, DynamicVars[nameof(ArtifactPower)].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(ScorchPower)].UpgradeValueBy(2);
    }
}