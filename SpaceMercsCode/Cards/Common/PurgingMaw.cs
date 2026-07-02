using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class PurgingMaw() : SpaceMercsCard(3,
    CardType.Attack, CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5, ValueProp.Move),
        new PowerVar<HungerPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<HungerPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int hungerTotal = 0;
        
        AttackCommand attackCmd = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        foreach (DamageResult result in attackCmd.Results.First())
        {
            if (result.TotalDamage > 0) hungerTotal++;
        }
        
        foreach(Creature player in CombatState.PlayerCreatures){
            var selfDamage = await CreatureCmd
                .Damage(choiceContext, player, DynamicVars.Damage.BaseValue, ValueProp.Move, this);
            if (selfDamage.First().TotalDamage > 0) hungerTotal++;
        }

        await PowerCmd.Apply<HungerPower>(choiceContext, Owner.Creature, hungerTotal * DynamicVars[nameof(HungerPower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(HungerPower)].UpgradeValueBy(1);
    }
}