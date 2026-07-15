using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Keywords;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class FrigidFortification() : SpaceMercsCard(3,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<SlowedPower>(1),
        new PowerVar<FrostArmorPower>(1),
        new PowerVar<FrigidFortificationPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SlowedPower>(),
        HoverTipFactory.FromPower<FrostArmorPower>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        SpaceMercsKeywords.Exert,
        CardKeyword.Retain
    ];
    
    protected override HashSet<CardTag> CanonicalTags => [
        SpaceMercsTags.Slows
    ];

    public override bool HasDeterminationAbility => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<SlowedPower>(choiceContext, enemy, DynamicVars[nameof(SlowedPower)].BaseValue, Owner.Creature, this);
        }
        await PowerCmd.Apply<FrostArmorPower>(choiceContext, Owner.Creature, DynamicVars[nameof(FrostArmorPower)].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<FrigidFortificationPower>(choiceContext, Owner.Creature, DynamicVars[nameof(FrigidFortificationPower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(SlowedPower)].UpgradeValueBy(2);
    }
}