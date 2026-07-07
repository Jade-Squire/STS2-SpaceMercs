using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class ColdDarkGentle() : SpaceMercsCard(10,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(40, ValueProp.Move),
        new PowerVar<HungerPower>(10)
    ];

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card == this)
        {
            foreach (var enemy in CombatState.HittableEnemies)
            {
                if (enemy.HasPower<FrozenPower>())
                {
                    enemy.GetPower<FrozenPower>()?.FreezeRemoved += FreezeRemoved;
                    EnergyCost.SetThisCombat(0);
                    InvokeEnergyCostChanged();
                }
            }
        }
        return base.AfterCardEnteredCombat(card);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Targeting(play.Target)
            .Execute(choiceContext);
        await PowerCmd.Apply<HungerPower>(choiceContext, Owner.Creature, DynamicVars[nameof(HungerPower)].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(20);
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is FrozenPower)
        {
            ((FrozenPower)power).FreezeRemoved += FreezeRemoved;
            foreach (var enemy in CombatState.HittableEnemies)
            {
                if (enemy.HasPower<FrozenPower>())
                {
                    EnergyCost.SetThisCombat(0);
                    InvokeEnergyCostChanged();
                    break;
                }
            }
        }
        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }

    private void FreezeRemoved(FrozenPower power)
    {
        bool found = false;
        foreach (var enemy in CombatState.HittableEnemies)
        {
            if (enemy.HasPower<FrozenPower>() && enemy != power.Owner)
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            EnergyCost.SetThisCombat(EnergyCost.Canonical);
            InvokeEnergyCostChanged();
        }
    }
}