using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

public class HungeringFlame() : SpaceMercsCard(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<CurePower>(),
        HoverTipFactory.FromPower<HungerPower>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Ethereal
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int hungerPowerAmt = 0;
        int curePowerAmt = 0;
        if (Owner.HasPower<HungerPower>()) hungerPowerAmt += Owner.Creature.GetPowerAmount<HungerPower>();
        if (Owner.HasPower<CurePower>()) curePowerAmt += Owner.Creature.GetPowerAmount<CurePower>();
        int targetPowerAmt = (hungerPowerAmt + curePowerAmt)/2;
        if (curePowerAmt > 0 || hungerPowerAmt > 0)
        {
            if (hungerPowerAmt <= 0)
            {
                await PowerCmd.Apply<HungerPower>(choiceContext, Owner.Creature, targetPowerAmt, Owner.Creature,
                    this);
            }
            else
            {
                await PowerCmd.ModifyAmount(choiceContext, Owner.Creature.GetPower<HungerPower>(),
                    targetPowerAmt - hungerPowerAmt, Owner.Creature, this);
            }

            if (curePowerAmt <= 0)
            {
                await PowerCmd.Apply<CurePower>(choiceContext, Owner.Creature, targetPowerAmt, Owner.Creature, this);
            }
            else
            {
                await PowerCmd.ModifyAmount(choiceContext, Owner.Creature.GetPower<CurePower>(),
                    targetPowerAmt - curePowerAmt, Owner.Creature, this);
            }
        }

        await Cmd.CustomScaledWait(0.4f, 0.8f);
        PlayerCmd.EndTurn(Owner, false);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}