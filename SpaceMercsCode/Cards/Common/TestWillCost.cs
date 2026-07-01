using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.CombatState;
using SpaceMercs.SpaceMercsCode.Keywords;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class TestWillCost() : SpaceMercsCard(3,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override int CanonicalDeterminationCost => -1;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        SpaceMercsKeywords.Exert
    ];

    public override void AfterCreated()
    {
        Owner.PlayerCombatState.Cosmopaladin().DeterminationChanged += DeterminationChanged;
        base.AfterCreated();
    }

    public void DeterminationChanged(int oldValue, int newValue)
    {
        if (newValue > 0)
        {
            EnergyCost.SetThisCombat(0);
            SetDeterminationCostThisCombat(1);
        }
        else
        {
            EnergyCost.SetThisCombat(EnergyCost.Canonical);
            SetDeterminationCostThisCombat(0);
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        GD.Print("Current Determination: " + Owner.PlayerCombatState.Cosmopaladin().Determination);
    }

    protected override void OnUpgrade()
    {

    }
}