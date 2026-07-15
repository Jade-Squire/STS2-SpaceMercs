using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Keywords;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

public class EmptyAndUnmoving() : SpaceMercsCard(4,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    public override bool HasDeterminationAbility => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        SpaceMercsKeywords.Exert
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<HungerPower>(),
        HoverTipFactory.FromPower<FrostArmorPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<EmptyAndUnmovingPower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<EmptyAndUnmovingPower>(choiceContext, Owner.Creature,
            DynamicVars[nameof(EmptyAndUnmovingPower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}