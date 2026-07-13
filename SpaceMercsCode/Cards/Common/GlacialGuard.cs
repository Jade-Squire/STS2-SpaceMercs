using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class GlacialGuard() : SpaceMercsCard(2,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(15, ValueProp.Move),
        new PowerVar<GlacialGuardPower>(1),
        new PowerVar<GlacialGuardPlusPower>(5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SlowedPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await PowerCmd.Apply<GlacialGuardPower>(choiceContext, Owner.Creature,
            DynamicVars[nameof(GlacialGuardPower)].BaseValue, Owner.Creature, this);
        if (IsUpgraded)
        {
            await PowerCmd.Apply<GlacialGuardPlusPower>(choiceContext, Owner.Creature, DynamicVars[nameof(GlacialGuardPlusPower)].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {

    }
}