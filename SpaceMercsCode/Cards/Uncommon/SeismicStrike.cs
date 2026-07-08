using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class SeismicStrike() : SpaceMercsCard(0,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override bool HasEnergyCostX => true;

    protected override HashSet<CardTag> CanonicalTags => [
        CardTag.Strike
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new RefundVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<JoltPower>(),
        HoverTipFactory.FromPower<AmpPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int energyOnPlay = ResolveEnergyXValue();
        if (energyOnPlay <= 0)
            return;
        await DamageCmd.Attack(new DamageVar(energyOnPlay, ValueProp.Move).BaseValue)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Targeting(play.Target)
            .WithHitCount(2)
            .Execute(choiceContext);
        await PowerCmd.Apply<JoltPower>(choiceContext, play.Target, energyOnPlay * 2, Owner.Creature, this);
        await PowerCmd.Apply<AmpPower>(choiceContext, Owner.Creature, energyOnPlay * 2, Owner.Creature, this);
        if (IsUpgraded)
        {
            await CardPileCmd.Draw(choiceContext, energyOnPlay, Owner);
        }
    }

    protected override void OnUpgrade()
    {

    }
}