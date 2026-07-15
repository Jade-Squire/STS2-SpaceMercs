using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Commands;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class CantDieAgain() : SpaceMercsCard(3,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    private bool _hookedIntoHpChanged = false;
    
    public override bool GainsBlock => true;

    public override bool HasDeterminationAbility => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "SPACEMERCS-DETERMINATION.title"), new LocString("static_hover_tips", "SPACEMERCS-DETERMINATION.description"))
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("Determination", 4)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CosmopaladinPlayerCmd.GainDetermination(DynamicVars["Determination"].IntValue, Owner);
        int blockToGain = Owner.Creature.MaxHp - Owner.Creature.CurrentHp;
        if (!IsUpgraded)
        {
            blockToGain /= 2;
        }

        await CreatureCmd.GainBlock(Owner.Creature, blockToGain, ValueProp.Unpowered, play);
    }

    protected override void OnUpgrade()
    {
        
    }

    private void HpChanged(int oldHp, int newHp)
    {
        if (Owner.Creature.MaxHp / 2 > newHp)
        {
            EnergyCost.SetThisCombat(0);
        }
        else
        {
            EnergyCost.SetThisCombat(EnergyCost.Canonical);
        }
    }
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (!_hookedIntoHpChanged)
        {
            if (Owner.PlayerCombatState == null) return base.AfterCardEnteredCombat(card);
            Owner.Creature.CurrentHpChanged += HpChanged;
            _hookedIntoHpChanged = true;
        }
        return base.AfterCardEnteredCombat(card);
    }

    public override Task BeforeCombatStart()
    {
        if (!_hookedIntoHpChanged)
        {
            if (Owner.PlayerCombatState == null) return base.BeforeCombatStart();
            Owner.Creature.CurrentHpChanged += HpChanged;
            _hookedIntoHpChanged = true;
        }
        return base.BeforeCombatStart();
    }
}