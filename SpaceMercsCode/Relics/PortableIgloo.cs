using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards.Unique;
using SpaceMercs.SpaceMercsCode.Powers;
using SpaceMercs.SpaceMercsCode.Relics;

namespace SpaceMercs.SpaceMercsCode.Relics;

public class PortableIgloo() : SpaceMercsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<IceBlock>(),
        HoverTipFactory.FromPower<FrostArmorPower>()
    ];

    public override async Task BeforeCombatStart()
    {
        CardModel? card = Owner.Creature.CombatState?.CreateCard<IceBlock>(Owner);
        if (card is null)
        {
            return;
        }

        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }
}