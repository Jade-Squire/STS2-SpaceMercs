using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class PhoenixRising() : SpaceMercsCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    private int _currentCure = 1;
    private int _increasedCure;

    [SavedProperty]
    public int CurrentCure
    {
        get => _currentCure;
        set
        {
            AssertMutable();
            _currentCure = value;
            DynamicVars[nameof(CurePower)].BaseValue = _currentCure;
        }
    }

    [SavedProperty]
    public int IncreasedCure
    {
        get => _increasedCure;
        set
        {
            AssertMutable();
            _increasedCure = value;
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<CurePower>(1),
        new IntVar("CureIncrease", 1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<CurePower>(choiceContext, Owner.Creature, DynamicVars[nameof(CurePower)].BaseValue, Owner.Creature, this);
        BuffFromPlay(DynamicVars["CureIncrease"].IntValue);
        if (!(DeckVersion is PhoenixRising deckVersion))
        {
            return;
        }
        deckVersion.BuffFromPlay(DynamicVars["CureIncrease"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CureIncrease"].UpgradeValueBy(1);
    }

    protected override void AfterDowngraded() => UpdateStats();

    private void BuffFromPlay(int extraCure)
    {
        IncreasedCure += extraCure;
        UpdateStats();
    }
    
    private void UpdateStats()
    {
        this.CurrentCure = 1 + IncreasedCure;
    }
}