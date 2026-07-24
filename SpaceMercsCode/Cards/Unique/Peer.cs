using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Combat;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Unique;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class Peer() : SpaceMercsCard(0,
    CardType.Status, CardRarity.None,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<AmpPower>(5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<AmpPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<AmpPower>(choiceContext, Owner.Creature, DynamicVars[nameof(AmpPower)].BaseValue,
            Owner.Creature, this);
        
        List<CardModel> cardsToRemove = new();
        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card is Release or Shake)
            {
                cardsToRemove.Add(card);
            }
        }

        foreach (var card in cardsToRemove)
            await CardPileCmd.RemoveFromCombat(card);

        ReturnCardsToHand();

        await CardPileCmd.RemoveFromCombat(this);
    }
    
    private void ReturnCardsToHand()
    {
        List<CardModel> cardsToMove = new();
        foreach (var card in SpacemercsCustomPile.Aether.GetPile(Owner).Cards)
        {
            cardsToMove.Add(card);
        }
        
        foreach (var card in cardsToMove)
        {
            CardPileCmd.Add(card, PileType.Hand);
        }
    }
    
    public override Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        List<CardModel> cardsToRemove = new();
        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card is Release or Shake or Peer)
            {
                cardsToRemove.Add(card);
            }
        }

        foreach (var card in cardsToRemove)
        {
            CardPileCmd.RemoveFromCombat(card);
        }
        
        ReturnCardsToHand();
        return base.BeforeSideTurnEnd(choiceContext, side, participants);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(AmpPower)].UpgradeValueBy(5);
    }
}