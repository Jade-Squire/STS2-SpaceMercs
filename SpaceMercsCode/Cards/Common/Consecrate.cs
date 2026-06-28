using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class Consecrate() : SpaceMercsCard(2,
    CardType.Attack, CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(2, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int totalUnblocked = 0;
        AttackCommand cmd = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_slash")
            .TargetingAllOpponents(CombatState)
            .WithHitCount(2)
            .Execute(choiceContext);
        foreach (var res in cmd.Results)
        {
            foreach (var damage in res)
            {
                totalUnblocked += damage.UnblockedDamage;
            }
        }

        if (totalUnblocked > 0)
        {
            await CreatureCmd.Heal(Owner.Creature, totalUnblocked);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
    }
}