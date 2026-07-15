using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Rngs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Hooks;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class JoltPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && result.TotalDamage > 0)
        {
            Creature? randomTarget = GetFriendlyTargetOtherThanMe();
            int damageToDeal = Math.Min(result.TotalDamage, Amount);
            if (randomTarget != null)
            {
                Flash();
                SpaceMercsHooks.BeforeEnemyJolted(CombatState, choiceContext, randomTarget, dealer, cardSource, damageToDeal);
                await Cmd.CustomScaledWait(.2f, .4f);
                VfxCmd.PlayOnCreature(randomTarget, "vfx/vfx_attack_lightning");
                SfxCmd.Play("event:/sfx/characters/defect/defect_lightning_passive");
                PowerCmd.ModifyAmount(choiceContext, this, -damageToDeal, null, null);
                await CreatureCmd.Damage(choiceContext, randomTarget, new DamageVar(damageToDeal, ValueProp.Unpowered),
                    Owner, null);
                SpaceMercsHooks.AfterEnemyJolted(CombatState, choiceContext, randomTarget, dealer, cardSource, damageToDeal);
            }
        }
    }

    private Creature? GetFriendlyTargetOtherThanMe()
    {
        List<Creature> targets = Owner.CombatState.HittableEnemies.ToList();
        targets.Remove(Owner);
        Rng? rng;
        if (Owner.Monster == null)
        {
            return null;
        }
        rng = Owner.Monster.RunRng.CombatTargets;

        return rng.NextItem(targets);
    }
}