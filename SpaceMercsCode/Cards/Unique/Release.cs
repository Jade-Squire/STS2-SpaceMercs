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
using SpaceMercs.SpaceMercsCode.Relics;

namespace SpaceMercs.SpaceMercsCode.Cards.Unique;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class Release() : SpaceMercsCard(0,
    CardType.Status, CardRarity.None,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<AmpPower>(),
        HoverTipFactory.FromPower<JoltPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Owner.GetRelic<LightningInABottle>()?.Active = false;
        
        if (Owner.Creature.HasPower<AmpPower>())
        {
            await PowerCmd.Apply<AmpPower>(choiceContext, Owner.Creature,
                (IsUpgraded)
                    ? Owner.Creature.GetPower<AmpPower>().Amount * 2
                    : Owner.Creature.GetPower<AmpPower>().Amount,
                Owner.Creature, this);
        }

        foreach (var enemy in CombatState.HittableEnemies)
        {
            if (enemy.HasPower<JoltPower>())
            {
                await PowerCmd.Apply<JoltPower>(choiceContext, enemy,
                    (IsUpgraded) 
                        ? enemy.GetPower<JoltPower>().Amount * 2 
                        : enemy.GetPower<JoltPower>().Amount,
                    Owner.Creature, this);
            }
        }
        
        List<CardModel> cardsToRemove = new();
        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card is Peer or Shake)
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
        
    }
}