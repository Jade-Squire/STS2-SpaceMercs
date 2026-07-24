using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Hooks;
using SpaceMercs.SpaceMercsCode.Powers;
using SpaceMercs.SpaceMercsCode.Relics;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace SpaceMercs.SpaceMercsCode.Relics;

public class ShiningFork() : SpaceMercsRelic, IShouldLoseHunger
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<HungerPower>(), 
        HoverTipFactory.FromCard<Void>()
    ];

    public bool ShouldLoseHungerAtTurnStart() => false;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card is Void && card.Owner == Owner)
        {
            if (Owner.Creature.HasPower<HungerPower>())
            {
                await PowerCmd.Decrement(Owner.Creature.GetPower<HungerPower>());
            }
        }
    }

    public void AfterHungerLossPrevented()
    {
        Flash();
    }
}