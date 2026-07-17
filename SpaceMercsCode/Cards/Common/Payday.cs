using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class Payday() : SpaceMercsCard(0,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10, ValueProp.Move),
        new GoldVar(15),
        new IntVar("GoldCost", 15)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Fatal)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.LoseGold(DynamicVars["GoldCost"].BaseValue, Owner);
        var result = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Targeting(play.Target)
            .Execute(choiceContext);
        bool shouldTriggerFatal = play.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());

        if (result.Results.FirstOrDefault() is null)
        {
            return;
        }
        
        if (result.Results.FirstOrDefault()[0].WasTargetKilled && shouldTriggerFatal)
        {
            await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);
        }
    }

    protected override bool IsPlayable => Owner.Gold >= DynamicVars["GoldCost"].BaseValue;

    protected override void OnUpgrade()
    {
        DynamicVars.Gold.UpgradeValueBy(5);
    }
}