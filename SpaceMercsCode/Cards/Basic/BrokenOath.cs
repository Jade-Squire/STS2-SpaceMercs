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
using SpaceMercs.SpaceMercsCode.Cards.Rare;

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
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromCard<RememberedVow>()
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
            foreach (CardModel c in Owner.Deck.Cards)
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

    protected override void AfterDowngraded()
    {
        if (CostReduced)
        {
            EnergyCost.SetCustomBaseCost(1);
        }
        base.AfterDowngraded();
    }

    protected override void AfterDeserialized()
    {
        if (CostReduced)
        {
            EnergyCost.SetCustomBaseCost(1);
        }
    }

    public override void AfterTransformedFrom()
    {
        List<UnwaveringStarBase> bases = new();
        List<UnwaveringStarVow> vows = new();
        foreach (var card in Owner.Deck.Cards)
        {
            if (card is RememberedVow)
            {
                card.BeforeCardRemoved(this);
            }
            else if (card is UnwaveringStarBase)
            {
                bases.Add((UnwaveringStarBase)card);
            }
            else if (card is UnwaveringStarVow)
            {
                vows.Add((UnwaveringStarVow)card);
            }
        }

        foreach (var card in bases)
        {
            card.RemovedBrokenOath(this);
        }

        foreach (var card in vows)
        {
            card.RemovedBrokenOath(this);
        }
            
        base.AfterTransformedTo();
    }
}