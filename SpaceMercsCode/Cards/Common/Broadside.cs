using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class Broadside() : SpaceMercsCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(1, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int block = Owner.Creature.Block;
        await CreatureCmd.LoseBlock(Owner.Creature, (IsUpgraded) ? block / 2 : block);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_slash")
            .WithHitCount(block)
            .TargetingRandomOpponents(CombatState)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {

    }
}