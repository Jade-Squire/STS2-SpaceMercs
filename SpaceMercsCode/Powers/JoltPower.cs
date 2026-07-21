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
    
    private int _damageToDeal;
    private Creature? _dealer;
    private CardModel? _cardSource;

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (_damageToDeal > 0 && creature == Owner)
        {
            Creature? randomTarget = GetFriendlyTargetOtherThanMe();
            int damageToDeal = Math.Min(_damageToDeal, Amount);
            if (randomTarget != null)
            {
                Flash();
                await SpaceMercsHooks.BeforeEnemyJolted(CombatState, new ThrowingPlayerChoiceContext(), randomTarget, _dealer, _cardSource, damageToDeal);
                await Cmd.CustomScaledWait(.1f, .2f);
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -damageToDeal, null, null);
                VfxCmd.PlayOnCreature(randomTarget, "vfx/vfx_attack_lightning");
                SfxCmd.Play("event:/sfx/characters/defect/defect_lightning_passive");
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), randomTarget, new DamageVar(damageToDeal, ValueProp.Unpowered),
                    null, null);
                if(CombatState != null)
                    await SpaceMercsHooks.AfterEnemyJolted(CombatState, new ThrowingPlayerChoiceContext(), randomTarget, _dealer, _cardSource, damageToDeal);
            }
            _damageToDeal = 0;
            _dealer = null;
            _cardSource = null;
        }
    }

    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (target == Owner && amount > 0)
        {
            _damageToDeal = Math.Min((int)amount, Amount);
            _dealer = dealer;
            _cardSource = cardSource;
        }

        return base.ModifyHpLostAfterOsty(target, amount, props, dealer, cardSource);
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