using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Basic;

public class RememberedVow() : SpaceMercsCard(1,
    CardType.Attack, CardRarity.Basic,
    TargetType.AnyEnemy)
{
    private const string _increaseBlockKey = "IncreaseBlock";
    private const string _increaseDamageKey = "IncreaseDamage";

    private int _currentBlock = 1;
    private int _increasedBlock;
    private int _currentDamage = 1;
    private int _increasedDamage;

    public override bool GainsBlock => true;

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
}