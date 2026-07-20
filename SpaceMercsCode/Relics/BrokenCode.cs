using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Powers;
using SpaceMercs.SpaceMercsCode.Relics;

namespace SpaceMercs.SpaceMercsCode.Relics;

public class BrokenCode() : SpaceMercsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;

    private bool _triggeredThisTurn;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<AmpPower>()
    ];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (!_triggeredThisTurn && power is AmpPower && Owner.Creature == power.Owner && amount > 0)
        {
            _triggeredThisTurn = true;
            await CreatureCmd.GainBlock(Owner.Creature, amount, ValueProp.Unpowered, null);
        }
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Creature.Side)
        {
            _triggeredThisTurn = false;
        }
        return base.AfterSideTurnEnd(choiceContext, side, participants);
    }
}