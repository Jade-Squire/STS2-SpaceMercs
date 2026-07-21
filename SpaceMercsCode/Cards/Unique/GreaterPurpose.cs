using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Unique;

public class GreaterPurpose() : SpaceMercsCard(3,
    CardType.Power, CardRarity.Ancient,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("CostRequired", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        IEnumerable<CardModel> results = await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1),
            card => card.EnergyCost.GetWithModifiers(CostModifiers.All) <= DynamicVars["CostRequired"].IntValue, this);
        CardModel card = results.FirstOrDefault();
        if (card != null)
        {
            if (card is UnknownBase)
            {
                await CardPileCmd.RemoveFromCombat(card);
                card = ((UnknownBase)card).ActualCard;
            }
            await CardCmd.Exhaust(choiceContext, card);
            GreaterPurposePower power = (GreaterPurposePower)ModelDb.Power<GreaterPurposePower>().ToMutable();
            power.AddSelectedCard(card);
            await PowerCmd.Apply(choiceContext, power, Owner.Creature, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CostRequired"].UpgradeValueBy(1);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain
    ];
}