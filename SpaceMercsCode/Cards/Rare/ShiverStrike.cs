using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

public class ShiverStrike() : SpaceMercsCard(2,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move)
    ];

    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Strike
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        List<PowerModel> originalDebuffs = play.Target.Powers.Where(p => p.TypeForCurrentAmount == PowerType.Debuff).Select((Func<PowerModel, PowerModel>) (p => (PowerModel) p.ClonePreservingMutability())).ToList();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            if (enemy != play.Target)
            {
                foreach (PowerModel power in originalDebuffs)
                {
                    PowerModel instanceForStacking =
                        PowerCmd.FindExistingInstanceForStacking(power, enemy, power.Applier);
                    if (instanceForStacking != null)
                    {
                        Misery.DoHackyThingsForSpecificPowers(instanceForStacking);
                        await PowerCmd.ModifyAmount(choiceContext, instanceForStacking, power.Amount, power.Applier,
                            this);
                    }
                    else
                    {
                        PowerModel newPower = (PowerModel)power.ClonePreservingMutability();
                        Misery.DoHackyThingsForSpecificPowers(newPower);
                        await PowerCmd.Apply(choiceContext, newPower, enemy, power.Amount, power.Applier, this);
                    }
                }
            }
        }

    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
    }
}