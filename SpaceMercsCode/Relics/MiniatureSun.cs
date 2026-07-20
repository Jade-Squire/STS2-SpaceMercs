using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Powers;
using SpaceMercs.SpaceMercsCode.Relics;

namespace SpaceMercs.SpaceMercsCode.Relics;

public class MiniatureSun() : SpaceMercsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    private bool _triggeredThisCombat;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<CurePower>(10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<CurePower>()
    ];

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (!_triggeredThisCombat && creature == Owner.Creature && delta < 0)
        {
            _triggeredThisCombat = true;
            await PowerCmd.Apply<CurePower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars[nameof(CurePower)].BaseValue, Owner.Creature, null);
        }
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _triggeredThisCombat = false;
        return base.AfterCombatEnd(room);
    }
}