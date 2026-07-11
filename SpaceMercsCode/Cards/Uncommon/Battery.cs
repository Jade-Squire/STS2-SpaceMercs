using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class Battery() : SpaceMercsCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<AmpPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Decimal ampStored;
        if (Owner.Creature.HasPower<AmpPower>())
        {
            ampStored = Owner.Creature.GetPowerAmount<AmpPower>();
            if (IsUpgraded)
            {
                ampStored *= 2;
            }
            await PowerCmd.Apply<BatteryPower>(choiceContext, Owner.Creature, ampStored, Owner.Creature, this);
            await PowerCmd.Remove(Owner.Creature.GetPower<AmpPower>());
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}