using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class SuppressPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && result.UnblockedDamage > 0)
        {
            int vuln, weak;
            vuln = Owner.HasPower<VulnerablePower>() ? Owner.GetPowerAmount<VulnerablePower>() : 0;
            weak = Owner.HasPower<WeakPower>() ? Owner.GetPowerAmount<WeakPower>() : 0;
            if (vuln < weak)
            {
                PowerCmd.Apply<VulnerablePower>(choiceContext, Owner, 1, dealer, null);
            }
            else if (weak < vuln)
            {
                PowerCmd.Apply<WeakPower>(choiceContext, Owner, 1, dealer, null);
            }
            else
            {
                if (Owner.Monster.RunRng.Niche.NextBool())
                {
                    PowerCmd.Apply<WeakPower>(choiceContext, Owner, 1, dealer, null);
                }
                else
                {
                    PowerCmd.Apply<VulnerablePower>(choiceContext, Owner, 1, dealer, null);
                }
            }

            PowerCmd.Decrement(this);
        }
        
        return base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);
    }
}