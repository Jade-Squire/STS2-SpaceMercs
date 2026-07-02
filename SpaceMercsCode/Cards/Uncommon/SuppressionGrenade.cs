using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Keywords;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class SuppressionGrenade() : SpaceMercsCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<SuppressPower>(1)
    ];

    protected override HashSet<CardTag> CanonicalTags =>
    [
        SpaceMercsTags.Grenade
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SuppressPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int energyOnPlay = Owner.PlayerCombatState.Energy + play.Resources.EnergySpent;
        for (int i = 0; i < energyOnPlay; i++)
        {
            await PowerCmd.Apply<SuppressPower>(choiceContext, play.Target,
                DynamicVars[nameof(SuppressPower)].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        BaseReplayCount++;
    }
}