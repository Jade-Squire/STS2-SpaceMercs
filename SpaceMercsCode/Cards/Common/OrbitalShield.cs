using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class OrbitalShield() : SpaceMercsCard(3,
    CardType.Attack, CardRarity.Common,
    TargetType.RandomEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5, ValueProp.Move),
        new IntVar("Multihit", 3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int totalUnblocked = 0;
        
        AttackCommand result = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_blunt")
            .TargetingRandomOpponents(CombatState)
            .WithHitCount(DynamicVars["Multihit"].IntValue)
            .Execute(choiceContext);

        foreach (var res in result.Results)
        {
            totalUnblocked += res.First().UnblockedDamage;
        }
        
        await CreatureCmd.GainBlock(Owner.Creature, totalUnblocked, ValueProp.Move, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}