using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

public class ForgeMaster() : SpaceMercsCard(2,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ScorchPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ForgeMasterPower>(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<ForgeMasterPower>(choiceContext, Owner.Creature,
            DynamicVars[nameof(ForgeMasterPower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}