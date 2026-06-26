using System.Collections;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class SmokeScreen() : SpaceMercsCard(2,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(12, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        IEnumerable<CardModel> pile = [];
        foreach(var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (!card.EnergyCost.CostsX && card.EnergyCost.GetWithModifiers(CostModifiers.Local) > 0)
            {
                pile = pile.AddItem(card);
            }
        }

        if (pile == null)
        {
            return;
        }
        CardModel selectedCard = Owner.RunState.Rng.CombatCardSelection.NextItem(pile);
        if (selectedCard == null)
        {
            return;
        }
        selectedCard.EnergyCost.AddThisTurnOrUntilPlayed(-1);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}