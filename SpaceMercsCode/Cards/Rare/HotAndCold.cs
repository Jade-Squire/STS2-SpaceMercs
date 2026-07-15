using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Commands;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Hooks;
using SpaceMercs.SpaceMercsCode.Keywords;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

public class HotAndCold() : SpaceMercsCard(3,
    CardType.Skill, CardRarity.Rare,
    TargetType.AllEnemies), IEnemyIgnited, IEnemyFrozen
{
    private bool _currentlyResolving = false;

    public override bool HasDeterminationAbility => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain,
        CardKeyword.Exhaust,
        SpaceMercsKeywords.Exert
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ScorchPower>(),
        HoverTipFactory.FromPower<SlowedPower>(),
        HoverTipFactory.FromPower<FrozenPower>()
    ];
    
    protected override HashSet<CardTag> CanonicalTags => [
        SpaceMercsTags.Slows
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        _currentlyResolving = true;
        foreach (var enemy in CombatState.HittableEnemies)
        {
            if (enemy.HasPower<ScorchPower>())
            {
                await PowerCmd.Apply<ScorchPower>(choiceContext, enemy, enemy.GetPowerAmount<ScorchPower>(),
                    Owner.Creature, this);
            }

            if (enemy.HasPower<SlowedPower>())
            {
                await PowerCmd.Apply<SlowedPower>(choiceContext, enemy, enemy.GetPowerAmount<SlowedPower>(),
                    Owner.Creature, this);
            }
        }
        _currentlyResolving = false;
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }

    public async void AfterEnemyIgnited(PlayerChoiceContext choiceContext, Creature ignitedCreature, Creature? applier,
        CardModel? cardSource)
    {
        if (_currentlyResolving)
        {
            await CosmopaladinPlayerCmd.GainDetermination(1, Owner);
        }
    }

    public async void AfterEnemyFrozen(PlayerChoiceContext choiceContext, Creature frozenCreature, Creature? applier,
        CardModel? cardSource)
    {
        if (_currentlyResolving)
        {
            await CosmopaladinPlayerCmd.GainDetermination(1, Owner);
        }
    }
}