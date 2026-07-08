using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Keywords;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class AbyssalFlare() : SpaceMercsCard(3,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ScorchPower>(10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<CurePower>(),
        HoverTipFactory.FromPower<HungerPower>()
    ];

    public override bool GainsBlock => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (Owner.HasPower<CurePower>())
        {
            int curePower = Owner.Creature.GetPowerAmount<CurePower>();
            await PowerCmd.Remove<CurePower>(Owner.Creature);
            await CreatureCmd.GainBlock(Owner.Creature, new BlockVar(curePower, ValueProp.Unpowered), play);
            await PowerCmd.Apply<HungerPower>(choiceContext, Owner.Creature, curePower, Owner.Creature, this);
        }

        if (IsUpgraded)
        {
            Creature randCreature = Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
            if(randCreature != null)
                await PowerCmd.Apply<ScorchPower>(choiceContext, randCreature, DynamicVars[nameof(ScorchPower)].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(SpaceMercsKeywords.GainsScorch);
    }
}