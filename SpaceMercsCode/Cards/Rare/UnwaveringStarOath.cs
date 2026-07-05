using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class UnwaveringStarOath() : SpaceMercsCard(3,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SuppressPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(13, ValueProp.Move),
        new PowerVar<SuppressPower>(3)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        
        await PowerCmd.Apply<SuppressPower>(choiceContext, play.Target, DynamicVars[nameof(SuppressPower)].BaseValue,
            Owner.Creature, this);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Targeting(play.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
        DynamicVars[nameof(SuppressPower)].UpgradeValueBy(1);
    }
    
    public override Task BeforeCardRemoved(CardModel card)
    {
        if (Owner.Deck.Cards.Contains(this))
        {
            if (card is RememberedVow)
            {
                RemovedRememberedVow(card);
            }
        }
        return base.BeforeCardRemoved(card);
    }

    private void RemovedRememberedVow(CardModel cardRemoved)
    {
        // make sure theres no more vows
        foreach (var card in Owner.Deck.Cards)
        {
            if (card is RememberedVow && card != cardRemoved)
            {
                return;
            }
        }
        
        CardModel newCard = ModelDb.Card<AnswerTheCall>().ToMutable();
        newCard.Owner = Owner;
        if (IsUpgraded)
        {
            CardCmd.Upgrade(newCard, CardPreviewStyle.None);
        }

        CardCmd.Transform(this, newCard);
    }

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        if (card is BrokenOath)
        {
            CardModel endCard = ModelDb.Card<UnwaveringStarBase>().ToMutable();
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