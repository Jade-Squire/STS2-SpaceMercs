using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Powers;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace SpaceMercs.SpaceMercsCode.Relics;

public class OfTheVoid() : SpaceMercsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.None;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Void>(),
        HoverTipFactory.FromPower<HungerPower>()
    ];

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card is Void)
        {
            await CreatureCmd.Heal(Owner.Creature, 1);
            await PowerCmd.Apply<HungerPower>(choiceContext, Owner.Creature, 1, null, null);
        }
    }
}