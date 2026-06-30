using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class BurningTreadsPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        await PowerCmd.Apply<ScorchPower>(new ThrowingPlayerChoiceContext(), Owner, Amount, cardPlay.Card.Owner.Creature, null);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        PowerCmd.Remove<BurningTreadsPower>(Owner);
        return base.AfterSideTurnEnd(choiceContext, side, participants);
    }
}