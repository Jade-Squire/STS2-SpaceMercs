using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Hooks;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class ColdShoulder() : SpaceMercsCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self), IEnemyFrozen
{
    private bool _creatureIsFrozen = false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<IntangiblePower>(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FrozenPower>(),
        HoverTipFactory.FromPower<IntangiblePower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<IntangiblePower>(choiceContext, Owner.Creature,
            DynamicVars[nameof(IntangiblePower)].BaseValue, Owner.Creature, this);
    }

    protected override bool IsPlayable => _creatureIsFrozen;

    public void AfterEnemyFrozen(PlayerChoiceContext choiceContext, Creature frozenCreature, Creature? applier,
        CardModel? cardSource)
    {
        if (frozenCreature.HasPower<FrozenPower>())
        {
            _creatureIsFrozen = true;
            frozenCreature.GetPower<FrozenPower>().FreezeRemoved += FreezeRemoved;
            NCard.FindOnTable(this)?.UpdateVisuals(Pile.Type, CardPreviewMode.None);
        }
    }

    private void FreezeRemoved(FrozenPower power)
    {
        foreach (var enemy in CombatState.HittableEnemies)
        {
            if (enemy.HasPower<FrozenPower>())
            {
                return;
            }
        }
        _creatureIsFrozen = false;
        
        NCard.FindOnTable(this).UpdateVisuals(Pile.Type, CardPreviewMode.None);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}