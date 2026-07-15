using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Cards.Rare;

namespace SpaceMercs.SpaceMercsCode.Cards.Basic;

public class RememberedVow() : SpaceMercsCard(1,
    CardType.Attack, CardRarity.Basic,
    TargetType.AnyEnemy), IPermaScalingCard
{
    private const string _increaseBlockKey = "IncreaseBlock";
    private const string _increaseDamageKey = "IncreaseDamage";

    private int _currentBlock = 1;
    private int _increasedBlock;
    private int _currentDamage = 1;
    private int _increasedDamage;
    private bool _costReduced;

    public override bool GainsBlock => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<BrokenOath>()
    ];

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

    [SavedProperty]
    public int CurrentBlock
    {
        get => _currentBlock;
        set
        {
            AssertMutable();
            _currentBlock = value;
            DynamicVars.Block.BaseValue = _currentBlock;
        }
    }

    [SavedProperty]
    public int CurrentDamage
    {
        get => _currentDamage;
        set
        {
            AssertMutable();
            _currentDamage = value;
            DynamicVars.Damage.BaseValue = _currentDamage;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(CurrentBlock, ValueProp.Move),
        new DamageVar(CurrentDamage, ValueProp.Move),
        new IntVar(_increaseBlockKey, 1),
        new IntVar(_increaseDamageKey, 1)
    ];

    [SavedProperty]
    public int IncreasedBlock
    {
        get => _increasedBlock;
        set
        {
            AssertMutable();
            _increasedBlock = value;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get => _increasedDamage;
        set
        {
            AssertMutable();
            _increasedDamage = value;
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target!, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        BuffCard();
    }

    protected override void OnUpgrade()
    {
        
    }

    protected override void AfterDowngraded() => UpdateStats();

    private void BuffFromPlay(int extraBlock, int extraDamage)
    {
        IncreasedBlock += extraBlock;
        IncreasedDamage += extraDamage;
        UpdateStats();
    }

    private void UpdateStats()
    {
        this.CurrentDamage = 1 + IncreasedDamage;
        this.CurrentBlock = 1 + IncreasedBlock;
    }
    
    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        if (card is BrokenOath && card.Owner == Owner)
        {
            EnergyCost.SetCustomBaseCost(1);
            CostReduced = false;
        }
        return base.TryModifyCardBeingAddedToDeck(card, out newCard);
    }

    public override Task BeforeCardRemoved(CardModel card)
    {
        if (card is BrokenOath && card.Owner == Owner)
        {
            foreach (CardModel c in Owner.Deck.Cards)
            {
                if (c != card && c is BrokenOath)
                {
                    return base.BeforeCardRemoved(c);
                }
            }
            
            EnergyCost.SetCustomBaseCost(0);
            CostReduced = true;
        }
        return base.BeforeCardRemoved(card);
    }

    public override void AfterTransformedFrom()
    {
        List<UnwaveringStarBase> bases = new();
        List<UnwaveringStarOath> oaths = new();

        foreach (var card in Owner.Deck.Cards)
        {
            if (card is BrokenOath)
            {
                card.BeforeCardRemoved(this);
            }
            else if (card is UnwaveringStarBase)
            {
                bases.Add((UnwaveringStarBase)card);
            }
            else if (card is UnwaveringStarOath)
            {
                oaths.Add((UnwaveringStarOath)card);
            }
        }

        foreach (var card in bases)
        {
            card.RemovedRememberedVow(this);
        }

        foreach (var card in oaths)
        {
            card.RemovedRememberedVow(this);
        }
        base.AfterTransformedTo();
    }

    protected override void AfterDeserialized()
    {
        if (CostReduced)
        {
            EnergyCost.SetCustomBaseCost(0);
        }
    }

    public void BuffCard()
    {
        if (IsUpgraded)
        {
            BuffFromPlay(DynamicVars[_increaseBlockKey].IntValue, DynamicVars[_increaseDamageKey].IntValue);
            if (!(DeckVersion is RememberedVow deckVersion))
            {
                return;
            }
            deckVersion.BuffFromPlay(DynamicVars[_increaseBlockKey].IntValue, DynamicVars[_increaseDamageKey].IntValue);
        }
        else
        {
            if (Owner.RunState.Rng.Niche.NextBool())
            {
                BuffFromPlay(DynamicVars[_increaseBlockKey].IntValue, 0);
                if (!(DeckVersion is RememberedVow deckVersion))
                {
                    return;
                }
                deckVersion.BuffFromPlay(DynamicVars[_increaseBlockKey].IntValue, 0);
            }
            else
            {
                BuffFromPlay(0, DynamicVars[_increaseDamageKey].IntValue);
                if (!(DeckVersion is RememberedVow deckVersion))
                {
                    return;
                }
                deckVersion.BuffFromPlay(0, DynamicVars[_increaseDamageKey].IntValue);
            }
        }
    }
}