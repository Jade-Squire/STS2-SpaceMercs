using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Keywords;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

public class SturdyCircuitry() : SpaceMercsCard(3,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    public override bool HasDeterminationAbility => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<SturdyCircuitryPower>(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        SpaceMercsKeywords.Exert
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<SturdyCircuitryPower>(choiceContext, Owner.Creature,
            DynamicVars[nameof(SturdyCircuitryPower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(SturdyCircuitryPower)].UpgradeValueBy(1);
    }
}