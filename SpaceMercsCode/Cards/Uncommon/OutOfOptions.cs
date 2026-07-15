using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class OutOfOptions() : SpaceMercsCard(3,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FrozenPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override bool IsPlayable => !CombatManager.Instance.History.CardPlaysFinished.ToList()
        .FindAll(card => card.CardPlay.Card.Owner == Owner && card.HappenedThisTurn(CombatState)).Any();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<FrozenPower>(choiceContext, enemy, 1, Owner.Creature, this);
        }
        PlayerCmd.EndTurn(Owner, false);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}