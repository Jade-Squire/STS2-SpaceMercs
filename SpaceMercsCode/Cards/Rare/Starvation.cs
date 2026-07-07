using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class Starvation() : SpaceMercsCard(2,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StarvationPower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        StarvationPower? power = await PowerCmd.Apply<StarvationPower>(choiceContext, Owner.Creature,
            1, Owner.Creature, this);
        if (power != null)
        {
            power.AddIncreaseAmt(DynamicVars[nameof(StarvationPower)].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(StarvationPower)].UpgradeValueBy(1);
    }
}