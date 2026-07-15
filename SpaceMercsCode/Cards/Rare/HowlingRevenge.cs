using BaseLib.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

public class HowlingRevenge() : SpaceMercsCard(1,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(BaseLibKeywords.Purge)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        if (!IsUpgraded)
        {
            await PowerCmd.Apply<HowlingRevengePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<HowlingRevengePlusPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {

    }
}