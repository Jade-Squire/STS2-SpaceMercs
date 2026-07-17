using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using SpaceMercs.SpaceMercsCode.Cards.Unique;
using SpaceMercs.SpaceMercsCode.Combat;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class RimeOrReason() : SpaceMercsCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SlowedPower>(),
        HoverTipFactory.FromCard<Rime>(),
        HoverTipFactory.FromCard<Reason>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await RemoveOldRimeOrReasons();
        
        List<CardModel> cardsToMove = new();
        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            cardsToMove.Add(card);
        }

        foreach (var card in cardsToMove)
        {
            await CardPileCmd.Add(card, SpacemercsCustomPile.Aether);
        }

        Rime rime = CombatState.CreateCard<Rime>(Owner);
        Reason reason = CombatState.CreateCard<Reason>(Owner);

        if (IsUpgraded)
        {
            CardCmd.Upgrade(rime, CardPreviewStyle.None);
            CardCmd.Upgrade(reason, CardPreviewStyle.None);
        }
        
        await CardPileCmd.AddGeneratedCardToCombat(rime, PileType.Hand, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(reason, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {

    }

    private async Task RemoveOldRimeOrReasons()
    {
        List<CardModel> rimeOrReasons = new();
        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card is Rime or Reason)
            {
                rimeOrReasons.Add(card);
            }
        }
        foreach (var card in PileType.Draw.GetPile(Owner).Cards)
        {
            if (card is Rime or Reason)
            {
                rimeOrReasons.Add(card);
            }
        }
        foreach (var card in PileType.Discard.GetPile(Owner).Cards)
        {
            if (card is Rime or Reason)
            {
                rimeOrReasons.Add(card);
            }
        }
        foreach (var card in PileType.Play.GetPile(Owner).Cards)
        {
            if (card is Rime or Reason)
            {
                rimeOrReasons.Add(card);
            }
        }
        foreach (var card in PileType.Exhaust.GetPile(Owner).Cards)
        {
            if (card is Rime or Reason)
            {
                rimeOrReasons.Add(card);
            }
        }
        foreach (var card in SpacemercsCustomPile.Aether.GetPile(Owner).Cards)
        {
            if (card is Rime or Reason)
            {
                rimeOrReasons.Add(card);
            }
        }
        foreach (var card in rimeOrReasons)
        {
            await CardPileCmd.RemoveFromCombat(card);
        }
    }
}