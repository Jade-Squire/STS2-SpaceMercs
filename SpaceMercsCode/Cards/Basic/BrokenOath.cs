using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Basic;

public class BrokenOath() : SpaceMercsCard(2,
    CardType.Attack, CardRarity.Basic,
    TargetType.AnyEnemy)
{
    private bool _costReduced;

    [SavedProperty]
    public bool CostReduced
    {
        get => _costReduced;
        set
        {
            AssertMutable();
            _costReduced = value;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10, ValueProp.Move),
        new PowerVar<VulnerablePower>(2)
    ];

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        if (card is RememberedVow && card.Owner == Owner)
        {
            EnergyCost.SetCustomBaseCost(2);
            CostReduced = false;
        }
        return base.TryModifyCardBeingAddedToDeck(card, out newCard);
    }

    public override Task BeforeCardRemoved(CardModel card)
    {
        if (card is RememberedVow && card.Owner == Owner)
        {
            foreach (CardModel c in card.Pile.Cards)
            {
                if (c != card && c is RememberedVow)
                {
                    return base.BeforeCardRemoved(c);
                }
            }
            
            EnergyCost.SetCustomBaseCost(1);
            CostReduced = true;
        }
        return base.BeforeCardRemoved(card);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target!);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, DynamicVars[nameof(VulnerablePower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
        DynamicVars[nameof(VulnerablePower)].UpgradeValueBy(1);
    }
    
    protected override void AfterDeserialized()
    {
        if (CostReduced)
        {
            EnergyCost.SetCustomBaseCost(1);
        }
    }
}