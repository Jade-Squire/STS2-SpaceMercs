using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Cards.Rare;
using SpaceMercs.SpaceMercsCode.CombatState;
using SpaceMercs.SpaceMercsCode.Commands;
using SpaceMercs.SpaceMercsCode.Keywords;

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
    private bool _hookedIntoDetChanged = false;
    
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected virtual int CanonicalDeterminationCost => -1;
    public virtual bool HasDeterminationAbility => false;
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

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (!_hookedIntoDetChanged && Keywords.Contains(SpaceMercsKeywords.Exert))
        {
            if (Owner.PlayerCombatState == null) return base.AfterCardEnteredCombat(card);
            Owner.PlayerCombatState.Cosmopaladin().DeterminationChanged += DeterminationChanged;
            _hookedIntoDetChanged = true;
        }
        return base.AfterCardEnteredCombat(card);
    }

    public override Task BeforeCombatStart()
    {
        if (!_hookedIntoDetChanged && Keywords.Contains(SpaceMercsKeywords.Exert))
        {
            if (Owner.PlayerCombatState == null) return base.BeforeCombatStart();
            Owner.PlayerCombatState.Cosmopaladin().DeterminationChanged += DeterminationChanged;
            _hookedIntoDetChanged = true;
        }
        return base.BeforeCombatStart();
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
    
    public void TryToConvertDetermination()
    {
        var combatState = Owner.PlayerCombatState;
        if (combatState.Cosmopaladin().Determination >= 2)
        {
            PlayerCmd.GainEnergy(1, combatState._player);
            CosmopaladinPlayerCmd.LoseDetermination(2, combatState._player);
        }
    }

    public override bool TryModifyCardRewardOptions(Player player, List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        foreach (var rewardOptions in cardRewardOptions)
        {
            if (rewardOptions.Card is UnwaveringStarBase)
            {
                bool hasOath = false;
                bool hasVow = false;
                foreach (var card in rewardOptions.Card.Owner.Deck.Cards)
                {
                    if (card is BrokenOath)
                    {
                        hasOath = true;
                        if (hasVow)
                        {
                            return base.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions);
                        }
                    }
                    else if (card is RememberedVow)
                    {
                        hasVow = true;
                        if (hasOath)
                        {
                            return  base.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions);
                        }
                    }
                }

                if (!hasOath && hasVow)
                {
                    rewardOptions.ModifyCard(RunState.CreateCard<UnwaveringStarOath>(rewardOptions.Card.Owner));
                } 
                else if (hasOath && !hasVow)
                {
                    rewardOptions.ModifyCard(RunState.CreateCard<UnwaveringStarVow>(rewardOptions.Card.Owner));
                }
                else
                {
                    rewardOptions.ModifyCard(RunState.CreateCard<AnswerTheCall>(rewardOptions.Card.Owner));
                }
            } 
            else if (rewardOptions.Card is Indecisive)
            {
                bool hasOath = false;
                bool hasVow = false;
                foreach (var card in rewardOptions.Card.Owner.Deck.Cards)
                {
                    if (card is BrokenOath)
                    {
                        hasOath = true;
                        if (hasVow)
                        {
                            return base.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions);
                        }
                    }
                    else if (card is RememberedVow)
                    {
                        hasVow = true;
                        if (hasOath)
                        {
                            return base.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions);
                        }
                    }
                }

                if (!hasOath && hasVow)
                {
                    rewardOptions.ModifyCard(RunState.CreateCard<StandFirm>(rewardOptions.Card.Owner));
                } 
                else if (hasOath && !hasVow)
                {
                    rewardOptions.ModifyCard(RunState.CreateCard<ChillingPast>(rewardOptions.Card.Owner));
                }
                else
                {
                    rewardOptions.ModifyCard(RunState.CreateCard<NewPath>(rewardOptions.Card.Owner));
                }
            }
        }
        return base.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions);
    }

    public override void ModifyMerchantCardCreationResults(Player player, List<CardCreationResult> cards)
    {
        foreach (var shopCard in cards)
        {
            if (shopCard.Card is UnwaveringStarBase or UnwaveringStarVow or UnwaveringStarVow or AnswerTheCall)
            {
                bool hasOath = false;
                bool hasVow = false;
                foreach (var card in player.Deck.Cards)
                {
                    if (card is BrokenOath)
                    {
                        hasOath = true;
                        if (hasVow)
                        {
                            base.ModifyMerchantCardCreationResults(player, cards);
                            return;
                        }
                    }
                    else if (card is RememberedVow)
                    {
                        hasVow = true;
                        if (hasOath)
                        {
                            base.ModifyMerchantCardCreationResults(player, cards);
                            return;
                        }
                    }
                }

                if (hasOath && hasVow)
                {
                    if (shopCard.Card is not UnwaveringStarBase)
                    {
                        shopCard.ModifyCard(RunState.CreateCard<UnwaveringStarBase>(player));
                    }
                }
                else if (!hasOath && hasVow)
                {
                    shopCard.ModifyCard(RunState.CreateCard<UnwaveringStarOath>(player));
                } 
                else if (hasOath && !hasVow)
                {
                    shopCard.ModifyCard(RunState.CreateCard<UnwaveringStarVow>(player));
                }
                else
                {
                    shopCard.ModifyCard(RunState.CreateCard<AnswerTheCall>(player));
                }
            }
            else if (shopCard.Card is Indecisive or StandFirm or ChillingPast or NewPath)
            {
                bool hasOath = false;
                bool hasVow = false;
                foreach (var card in player.Deck.Cards)
                {
                    if (card is BrokenOath)
                    {
                        hasOath = true;
                        if (hasVow)
                        {
                            base.ModifyMerchantCardCreationResults(player, cards);
                            return;
                        }
                    }
                    else if (card is RememberedVow)
                    {
                        hasVow = true;
                        if (hasOath)
                        {
                            base.ModifyMerchantCardCreationResults(player, cards);
                            return;
                        }
                    }
                }

                if (hasOath && hasVow)
                {
                    if (shopCard.Card is not Indecisive)
                    {
                        shopCard.ModifyCard(RunState.CreateCard<Indecisive>(player));
                    }
                }
                else if (!hasOath && hasVow)
                {
                    shopCard.ModifyCard(RunState.CreateCard<StandFirm>(player));
                } 
                else if (hasOath && !hasVow)
                {
                    shopCard.ModifyCard(RunState.CreateCard<ChillingPast>(player));
                }
                else
                {
                    shopCard.ModifyCard(RunState.CreateCard<NewPath>(player));
                }
            }
        }
        base.ModifyMerchantCardCreationResults(player, cards);
    }
}