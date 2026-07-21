using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Basic;

public class KeptPromise() : SpaceMercsCard(2,
    CardType.Attack, CardRarity.Ancient,
    TargetType.AllEnemies), IPermaScalingCard
{
    private const string _increaseBlockKey = "IncreaseBlock";
    private const string _increaseDamageKey = "IncreaseDamage";

    public override bool GainsBlock => true;

    [SavedProperty]
    public int CurrentBlock
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            DynamicVars.Block.BaseValue = field;
        }
    } = 1;
    
    [SavedProperty]
    public int CurrentDamage
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            DynamicVars.Damage.BaseValue = field;
        }
    } = 1;
    
    [SavedProperty]
    public int IncreasedBlock
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(CurrentBlock, ValueProp.Move),
        new DamageVar(CurrentDamage, ValueProp.Move),
        new IntVar(_increaseBlockKey, 2),
        new IntVar(_increaseDamageKey, 2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        BuffCard();
    }

    protected override void OnUpgrade()
    {
        
    }
    
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
    
    protected override void AfterDowngraded() => UpdateStats();

    public void BuffCard()
    {
        if (IsUpgraded)
        {
            BuffFromPlay(DynamicVars[_increaseBlockKey].IntValue, DynamicVars[_increaseDamageKey].IntValue);
            if (!(DeckVersion is KeptPromise deckVersion))
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
                if (!(DeckVersion is KeptPromise deckVersion))
                {
                    return;
                }
                deckVersion.BuffFromPlay(DynamicVars[_increaseBlockKey].IntValue, 0);
            }
            else
            {
                BuffFromPlay(0, DynamicVars[_increaseDamageKey].IntValue);
                if (!(DeckVersion is KeptPromise deckVersion))
                {
                    return;
                }
                deckVersion.BuffFromPlay(0, DynamicVars[_increaseDamageKey].IntValue);
            }
        }
    }
}