using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Combat;

namespace SpaceMercs.SpaceMercsCode.Cards.Unique;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class Bottle() : SpaceMercsCard(0,
    CardType.Skill, CardRarity.None,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await RemoveOldBottles();

        List<CardModel> cardsToMove = new();
        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            cardsToMove.Add(card);
        }

        foreach (var card in cardsToMove)
        {
            await CardPileCmd.Add(card, SpacemercsCustomPile.Aether);
        }
        
        Shake shake = CombatState.CreateCard<Shake>(Owner);
        Peer peer = CombatState.CreateCard<Peer>(Owner);
        Release release = CombatState.CreateCard<Release>(Owner);
        
        await CardPileCmd.AddGeneratedCardToCombat(shake, PileType.Hand, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(peer, PileType.Hand, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(release, PileType.Hand, Owner);
    }

    private async Task RemoveOldBottles()
    {
        List<CardModel> bottles = new();
        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card is Shake or Peer or Release)
            {
                bottles.Add(card);
            }
        }

        foreach (var card in PileType.Draw.GetPile(Owner).Cards)
        {
            if (card is Shake or Peer or Release)
            {
                bottles.Add(card);
            }
        }

        foreach (var card in PileType.Discard.GetPile(Owner).Cards)
        {
            if (card is Shake or Peer or Release)
            {
                bottles.Add(card);
            }
        }

        foreach (var card in PileType.Exhaust.GetPile(Owner).Cards)
        {
            if (card is Shake or Peer or Release)
            {
                bottles.Add(card);
            }
        }

        foreach (var card in PileType.Play.GetPile(Owner).Cards)
        {
            if (card is Shake or Peer or Release)
            {
                bottles.Add(card);
            }
        }

        foreach (var card in SpacemercsCustomPile.Aether.GetPile(Owner).Cards)
        {
            if (card is Shake or Peer or Release)
            {
                bottles.Add(card);
            }
        }

        foreach (var card in bottles)
        {
            await CardPileCmd.RemoveFromCombat(card);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}