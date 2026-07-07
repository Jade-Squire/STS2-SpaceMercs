using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

public class DarkGift() : SpaceMercsCard(6,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new MaxHpVar(3)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        RelicModel relic;
        if (IsUpgraded)
        {
            relic = RelicFactory.PullNextRelicFromFront(Owner).ToMutable();
        }
        else
        {
            relic = RelicFactory.PullNextRelicFromFront(Owner, RelicRarity.Common).ToMutable();
        }

        await RelicCmd.Obtain(relic, Owner);
        await CreatureCmd.LoseMaxHp(choiceContext, Owner.Creature, DynamicVars.MaxHp.BaseValue, true);
    }

    protected override void OnUpgrade()
    {

    }
}