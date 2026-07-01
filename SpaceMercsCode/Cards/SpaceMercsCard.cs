using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using SpaceMercs.SpaceMercsCode.CombatState;

namespace SpaceMercs.SpaceMercsCode.Cards;

/*******************************************************************************************************************
 * Credit to Blizzarre's Runesmith2 for this custom alt energy cost (https://github.com/Blizzarre/Runesmith2-StS2) *
 *******************************************************************************************************************/

/// <summary>
/// This is the base class for your mod's cards, which is set up to load the card's images from your mod's resources.
/// When creating a card, right click the Cards folder and create a new file with the Custom Card template.
/// This Determination generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
/// </summary>
[Pool(typeof(CosmopaladinCardPool))]
public abstract class SpaceMercsCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    private bool _DeterminationCostSet;
    private int _baseDeterminationCost;
    public event Action? DeterminationCostChanged;
    private List<TemporaryCardCost> _temporaryDeterminationCosts = new();
    private int _lastDeterminationSpent;
    private bool _wasDeterminationCostJustUpgraded;
    
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it Determination simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected virtual int CanonicalDeterminationCost => -1;
    public virtual bool HasDeterminationCostX => false;
    public bool WasDeterminationCostJustUpgraded => _wasDeterminationCostJustUpgraded;

    public int BaseDeterminationCost
    {
        get
        {
            if (!IsMutable)
            {
                return CanonicalDeterminationCost;
            }
            if (!_DeterminationCostSet)
            {
                _baseDeterminationCost = CanonicalDeterminationCost;
                _DeterminationCostSet = true;
            }
            return _baseDeterminationCost;
        }
        private set
        {
            AssertMutable();
            if (!HasDeterminationCostX)
            {
                _baseDeterminationCost = value;
                _DeterminationCostSet = true;
            }
            Action determinationCostChanged = DeterminationCostChanged;
            if (determinationCostChanged == null)
            {
                return;
            }
            determinationCostChanged();
        }
    }

    public TemporaryCardCost? TemporaryDeterminationCosts
    {
        get => _temporaryDeterminationCosts.LastOrDefault();
    }

    public virtual int CurrentDeterminationCost
    {
        get
        {
            int? cost = _temporaryDeterminationCosts.LastOrDefault()?.Cost;
            if (!cost.HasValue)
                return BaseDeterminationCost;
            int? DeterminationCost = cost;
            return DeterminationCost.GetValueOrDefault() == 0 & DeterminationCost.HasValue && BaseDeterminationCost < 0 ? BaseDeterminationCost : DeterminationCost.Value;
        }
    }

    public int LastDeterminationSpent
    {
        get => _lastDeterminationSpent;
        set
        {
            AssertMutable();
            _lastDeterminationSpent = value;
        }
    }

    public int ResolveDeterminationXValue()
    {
        if (!HasDeterminationCostX)
            throw new InvalidOperationException("This card does not have an Determination X-cost");
        return Hook.ModifyXValue(CombatState, this, _lastDeterminationSpent);
    }

    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        _temporaryDeterminationCosts = _temporaryDeterminationCosts.ToList();
    }

    public void SetDeterminationCostUntilPlayed(int cost)
    {
        AddTemporaryDeterminationCost(TemporaryCardCost.UntilPlayed(cost));
    }

    public void SetDeterminationCostThisTurn(int cost)
    {
        AddTemporaryDeterminationCost(TemporaryCardCost.ThisTurn(cost));
    }

    public void SetDeterminationCostThisCombat(int cost)
    {
        AddTemporaryDeterminationCost(TemporaryCardCost.ThisCombat(cost));
    }

    public int GetDeterminationCostThisCombat()
    {
        TemporaryCardCost tempCost = _temporaryDeterminationCosts.FirstOrDefault((Func<TemporaryCardCost, bool>) (cost => cost != null && !cost.ClearsWhenTurnEnds && !cost.ClearsWhenCardIsPlayed));
        return tempCost == null ? BaseDeterminationCost : tempCost.Cost;
    }

    public void AddTemporaryDeterminationCost(TemporaryCardCost cost)
    {
        AssertMutable();
        _temporaryDeterminationCosts.Add(cost);
        Action determinationCostChanged = DeterminationCostChanged;
        if (determinationCostChanged == null)
            return;
        determinationCostChanged();
    }

    protected void UpgradeDeterminationCostBy(int addend)
    {
        if(HasDeterminationCostX)
            throw new InvalidOperationException($"UpgradeDeterminationCostBy called on {Id.Entry} which has star cost X.");
        if (addend == 0)
            return;
        int baseDeterminationCost = BaseDeterminationCost;
        BaseDeterminationCost += addend;
        _wasDeterminationCostJustUpgraded = true;
        if (BaseDeterminationCost >= baseDeterminationCost)
            return;
        _temporaryDeterminationCosts.RemoveAll((Predicate<TemporaryCardCost>) (c => c.Cost > BaseDeterminationCost));
    }

    public int GetDeterminationCostWithModifiers()
    {
        if (HasDeterminationCostX)
        {
            PlayerCombatState playerCombatState = Owner.PlayerCombatState;
            return playerCombatState == null ? 0 : playerCombatState.Cosmopaladin().Determination;
        }

        /* in case i want to have a hook to modify Determination cost
        CardPile pile = Pile;
        return pile != null && pile.IsCombatPile && CombatState != null ? SpaceMercsHook.ModifyDeterminationCost(CombatState, this, CurrentDeterminationCost) : CurrentDeterminationCost; */
        return CurrentDeterminationCost;
    }

    public Task SpendDetermination(int amount)
    {
        LastDeterminationSpent = amount;
        if (amount <= 0)
            return Task.CompletedTask;
        Owner.PlayerCombatState?.Cosmopaladin()?.LoseDetermination(amount);
        return Task.CompletedTask;
        /* in case i want to have a hook after Determination spent
        await SpaceMercsHook.AfterDeterminationSpent(Owner.Creature.CombatState, amount, Owner);*/
    }
}