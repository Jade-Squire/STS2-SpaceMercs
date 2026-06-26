using BaseLib.Patches.Localization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class ThrowingHammer() : SpaceMercsCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CurePower>(),
        HoverTipFactory.FromPower<ScorchPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3M, ValueProp.Move),
        new PowerVar<CurePower>(2),
        new PowerVar<StrengthPower>(0),
        new PowerVar<ScorchPower>(7)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target!, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        if (IsUpgraded)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars[nameof(StrengthPower)].BaseValue, Owner.Creature, this);
        }
        await PowerCmd.Apply<CurePower>(choiceContext, Owner.Creature, DynamicVars[nameof(CurePower)].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<ScorchPower>(choiceContext, play.Target, DynamicVars[nameof(ScorchPower)].BaseValue, play.Target, this);
        await CardPileCmd.Add(this, IsUpgraded ? PileType.Hand : PileType.Draw, IsUpgraded ? CardPilePosition.Bottom : CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(StrengthPower)].UpgradeValueBy(1);
        //HoverTips.AddItem(HoverTipFactory.FromPower<StrengthPower>());
    }
}