using System.Collections;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Commands;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

public class FinalStand() : SpaceMercsCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "SPACEMERCS-DETERMINATION.title"), new LocString("static_hover_tips", "SPACEMERCS-DETERMINATION.description"))
    ];

    public override bool HasDeterminationAbility => true;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (IsUpgraded)
        {
            List<CardModel> list = (await CardSelectCmd.FromHandForDiscard(choiceContext, Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, 0, 999999999), null, this)).ToList();
            await CardCmd.DiscardAndDraw(choiceContext, list,
                CardPile.MaxCardsInHand - PileType.Hand.GetPile(Owner).Cards.Count + list.Count);
        }
        else
        {
            await CardPileCmd.Draw(choiceContext, CardPile.MaxCardsInHand - PileType.Hand.GetPile(Owner).Cards.Count,
                Owner);
        }

        await PlayerCmd.GainEnergy(3, Owner);
        await CosmopaladinPlayerCmd.GainDetermination(3, Owner);
    }

    protected override bool IsPlayable => (double)Owner.Creature.CurrentHp /  Owner.Creature.MaxHp < 0.25;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override void OnUpgrade()
    {
        
    }
}