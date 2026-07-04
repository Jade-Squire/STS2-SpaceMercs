using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Character;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class AnswerTheCall() : SpaceMercsCard(5,
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
            CardCmd.TransformTo<UnwaveringStarOath>(this);
        }
        else if (card is BrokenOath)
        {
            CardCmd.TransformTo<UnwaveringStarVow>(this);
        }
        return base.TryModifyCardBeingAddedToDeck(card, out newCard);
    }
}