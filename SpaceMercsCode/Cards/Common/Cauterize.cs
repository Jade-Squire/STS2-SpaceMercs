using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class Cauterize() : SpaceMercsCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ScorchPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ScorchPower>(3),
        new PowerVar<CurePower>(0)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int energyOnPlay = ResolveEnergyXValue();
        await PowerCmd.Apply<ScorchPower>(choiceContext, play.Target,
            DynamicVars[nameof(ScorchPower)].BaseValue * energyOnPlay, Owner.Creature, this);
        if (IsUpgraded)
        {
            await PowerCmd.Apply<CurePower>(choiceContext, Owner.Creature,
                DynamicVars[nameof(CurePower)].BaseValue * energyOnPlay, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(CurePower)].UpgradeValueBy(3);
    }
}