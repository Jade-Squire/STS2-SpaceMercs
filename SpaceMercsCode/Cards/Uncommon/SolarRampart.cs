using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class SolarRampart() : SpaceMercsCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(4, ValueProp.Move),
        new PowerVar<CurePower>(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ HoverTipFactory.FromPower<CurePower>() ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, play);
        await PowerCmd.Apply<CurePower>(choiceContext, Owner.Creature, DynamicVars[nameof(CurePower)].BaseValue,
            Owner.Creature, this);
    }

    protected override bool ShouldGlowGoldInternal => PrevWasSkill();

    protected override bool IsPlayable => PrevWasSkill();

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(CurePower)].UpgradeValueBy(3);
    }

    private bool PrevWasSkill()
    {
        if (!CombatManager.Instance.History.CardPlaysFinished.Any())
        {
            return false;
        }

        CardModel prevCard = CombatManager.Instance.History.CardPlaysFinished.Last().CardPlay.Card;
        return prevCard.Type == CardType.Skill;
    }
}