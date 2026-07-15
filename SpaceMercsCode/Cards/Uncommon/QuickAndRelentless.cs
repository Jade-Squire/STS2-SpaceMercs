using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class QuickAndRelentless() : SpaceMercsCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<QuickAndRelentlessPower>(1),
        new PowerVar<QuickAndRelentlessPlusPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SlowedPower>(),
        HoverTipFactory.FromPower<JoltPower>()
    ];
    
    protected override HashSet<CardTag> CanonicalTags => [
        SpaceMercsTags.Slows
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<QuickAndRelentlessPower>(choiceContext, Owner.Creature,
            DynamicVars[nameof(QuickAndRelentlessPower)].BaseValue, Owner.Creature, this);
        if (IsUpgraded)
        {
            await PowerCmd.Apply<QuickAndRelentlessPlusPower>(choiceContext, Owner.Creature,
                DynamicVars[nameof(QuickAndRelentlessPlusPower)].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        
    }
}