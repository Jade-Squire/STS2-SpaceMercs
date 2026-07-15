using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Commands;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class ColdStare() : SpaceMercsCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<SlowedPower>(2),
        new IntVar("Determination", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SlowedPower>(),
        new HoverTip(new LocString("static_hover_tips", "SPACEMERCS-DETERMINATION.title"), new LocString("static_hover_tips", "SPACEMERCS-DETERMINATION.description"))
    ];

    public override bool HasDeterminationAbility => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Creature randCreature = Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (randCreature != null)
        {
            await PowerCmd.Apply<SlowedPower>(choiceContext, randCreature, DynamicVars[nameof(SlowedPower)].BaseValue, Owner.Creature, this);
        }

        await CosmopaladinPlayerCmd.GainDetermination(DynamicVars["Determination"].IntValue, Owner, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Determination"].UpgradeValueBy(1);
    }
}