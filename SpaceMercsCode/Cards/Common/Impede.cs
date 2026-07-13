using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class Impede() : SpaceMercsCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(4, ValueProp.Move),
        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedVar("CardsPlayed").WithMultiplier((card, _) => CombatManager.Instance.History.CardPlaysFinished.ToList().FindAll(entry => entry.HappenedThisTurn(card.Owner.Creature.CombatState)).Count)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int cardsPlayed;
        if (!CombatManager.Instance.History.CardPlaysFinished.Any())
        {
            cardsPlayed = 0;
        }
        else
        {
            cardsPlayed = CombatManager.Instance.History.CardPlaysFinished.ToList()
                .FindAll(entry => entry.HappenedThisTurn(CombatState)).Count;
        }

        for (int i = 0; i < cardsPlayed; i++)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1);
    }
}