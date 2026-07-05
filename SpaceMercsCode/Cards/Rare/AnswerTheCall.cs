using BaseLib.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Relics;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class AnswerTheCall() : SpaceMercsCard(5,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromRelic<OfTheVoid>().ElementAt(0)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        BaseLibKeywords.Purge
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await RelicCmd.Obtain<OfTheVoid>(Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-5);
    }

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        if (card is RememberedVow)
        {
            CardModel endCard = ModelDb.Card<UnwaveringStarOath>().ToMutable();
            endCard.Owner = Owner;
            if (IsUpgraded)
            {
                CardCmd.Upgrade(endCard, CardPreviewStyle.None);
            }

            CardCmd.Transform(this, endCard);
        }
        else if (card is BrokenOath)
        {
            CardModel endCard = ModelDb.Card<UnwaveringStarVow>().ToMutable();
            endCard.Owner = Owner;
            if (IsUpgraded)
            {
                CardCmd.Upgrade(endCard, CardPreviewStyle.None);
            }

            CardCmd.Transform(this, endCard);
        }
        return base.TryModifyCardBeingAddedToDeck(card, out newCard);
    }
}