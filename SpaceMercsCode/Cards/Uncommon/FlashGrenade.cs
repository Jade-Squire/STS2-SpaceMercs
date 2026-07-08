using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class FlashGrenade() : SpaceMercsCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.RandomEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [
        SpaceMercsTags.Grenade
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<JoltPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<JoltPower>(9),
        new IntVar("NumTargets", 1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        List<Creature> targets = GetTargets(DynamicVars["NumTargets"].IntValue);
        foreach (var c in targets)
        {
            await PowerCmd.Apply<JoltPower>(choiceContext, c, DynamicVars[nameof(JoltPower)].BaseValue, Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NumTargets"].UpgradeValueBy(1);
    }

    private List<Creature> GetTargets(int numTargets)
    {
        List<Creature> ret = new();
        for (int i = 0; i < numTargets; i++)
        {
            List<Creature> pool = CombatState.HittableEnemies.ToList();
            if (!pool.Any())
                return ret;
            foreach (var c in ret)
            {
                if (pool.Contains(c))
                {
                    pool.Remove(c);
                }
            }

            Creature? target = Owner.RunState.Rng.CombatTargets.NextItem(pool);
            if (target == null)
                return ret;
            ret.Add(target);
        }

        return ret;
    }
}