using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Enums;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Rare;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class UnwaveringStarVow() : SpaceMercsCard(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self), IPermaScalingCard
{
    public override bool GainsBlock => true;

    [SavedProperty]
    public int CurrentBlock
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            DynamicVars.Block.BaseValue = field;
        }
    } = 1;

    [SavedProperty]
    public int IncreasedBlock
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    [SavedProperty]
    public int CurrentHunger
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            DynamicVars[nameof(HungerPower)].BaseValue = field;
        }
    } = 1;

    [SavedProperty]
    public int IncreasedHunger
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(CurrentBlock, ValueProp.Move),
        new PowerVar<HungerPower>(CurrentHunger)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await PowerCmd.Apply<HungerPower>(choiceContext, Owner.Creature, DynamicVars[nameof(HungerPower)].BaseValue,
            Owner.Creature, this);

        BuffCard();
    }

    protected override void OnUpgrade()
    {

    }

    public override Task BeforeCardRemoved(CardModel card)
    {
        if (Owner.Deck.Cards.Contains(this))
        {
            if (card is BrokenOath)
            {
                RemovedBrokenOath(card);
            }
        }

        return base.BeforeCardRemoved(card);
    }

    public void RemovedBrokenOath(CardModel cardRemoved)
    {
        // make sure theres no more vows
        foreach (var card in Owner.Deck.Cards)
        {
            if (card is BrokenOath && card != cardRemoved)
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
        if (card is RememberedVow)
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

    private void BuffFromPlay(int extraBlock, int extraHunger)
    {
        IncreasedBlock += extraBlock;
        IncreasedHunger += extraHunger;
        UpdateStats();
    }

    private void UpdateStats()
    {
        CurrentBlock = 1 + IncreasedBlock;
        CurrentHunger = 1 + IncreasedHunger;
    }

    public void BuffCard()
    {
        if (IsUpgraded)
        {
            BuffFromPlay(1, 1);
            if (!(DeckVersion is UnwaveringStarVow deckVersion))
            {
                return;
            }
            deckVersion.BuffFromPlay(1, 1);
        }
        else if (Owner.RunState.Rng.Niche.NextBool())
        {
            BuffFromPlay(1, 0);
            if (!(DeckVersion is UnwaveringStarVow deckVersion))
            {
                return;
            }
            deckVersion.BuffFromPlay(1, 0);
        }
        else
        {
            BuffFromPlay(0, 1);
            if (!(DeckVersion is UnwaveringStarVow deckVersion))
            {
                return;
            }
            deckVersion.BuffFromPlay(0, 1);
        }
    }
}