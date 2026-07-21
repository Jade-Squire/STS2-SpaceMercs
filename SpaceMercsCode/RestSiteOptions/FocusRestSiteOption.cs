using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards.Unique;

namespace SpaceMercs.SpaceMercsCode.RestSiteOptions;

public class FocusRestSiteOption(Player owner) : CustomRestSiteOption(owner)
{
    public override async Task<bool> OnSelect()
    {
        List<CardModel> cardsToRemove = new();
        foreach (var card in Owner.Deck.Cards)
        {
            if (card is Overwhelmed)
            {
                cardsToRemove.Add(card);
            }
        }

        foreach (var card in cardsToRemove)
        {
            await CardPileCmd.RemoveFromDeck(card);
        }
        return await Task.FromResult(true);
    }

    public override string OptionId => "SPACEMERCS-FOCUS";

    public override string CustomIconPath => "res://SpaceMercs/images/ui/rest_site/option_toke.png";
}