using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Character;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class NewPath() : SpaceMercsCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        
    }

    protected override void OnUpgrade()
    {

    }
    
    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        if (card is RememberedVow)
        {
            CardModel endCard = ModelDb.Card<StandFirm>().ToMutable();
            endCard.Owner = Owner;
            if (IsUpgraded)
            {
                CardCmd.Upgrade(endCard, CardPreviewStyle.None);
            }

            CardCmd.Transform(this, endCard);
        }
        else if (card is BrokenOath)
        {
            CardModel endCard = ModelDb.Card<ChillingPast>().ToMutable();
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