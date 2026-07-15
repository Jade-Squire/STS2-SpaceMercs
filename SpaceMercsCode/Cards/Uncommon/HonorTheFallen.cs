using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class HonorTheFallen() : SpaceMercsCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self), IPermaScalingCard
{
    public override bool GainsBlock => true;

    [SavedProperty]
    public int CurrentBlock
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            DynamicVars.Block.BaseValue = field;
        }
    }

    [SavedProperty]
    public int IncreasedBlock
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(CurrentBlock, ValueProp.Move),
        new IntVar("BlockIncrease", 2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature.IsEnemy && !wasRemovalPrevented)
        {
            BuffCard();
        }
        return base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
    }

    private void BuffStats(int extraBlock)
    {
        IncreasedBlock += extraBlock;
        UpdateStats();
    }

    private void UpdateStats()
    {
        CurrentBlock = 1 + IncreasedBlock;
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }

    public void BuffCard()
    {
        BuffStats(DynamicVars["BlockIncrease"].IntValue);
        if (!(DeckVersion is HonorTheFallen deckVersion))
        {
            return;
        }
        deckVersion.BuffStats(DynamicVars["BlockIncrease"].IntValue);
    }
}