using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Commands;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class BlissfulIgnorance() : SpaceMercsCard(3,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.SingleplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(20, ValueProp.Unpowered),
        new IntVar("Determination", 3)
    ];

    public override bool GainsBlock => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "SPACEMERCS-DETERMINATION.title"), new LocString("static_hover_tips", "SPACEMERCS-DETERMINATION.description"))
    ];

    public override bool HasDeterminationAbility => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CosmopaladinPlayerCmd.GainDetermination(DynamicVars["Determination"].IntValue, Owner, play);
        await PowerCmd.Apply<BlissfulIgnorancePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars["Determination"].UpgradeValueBy(2);
    }
}