using System.Collections;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Random;
using SpaceMercs.SpaceMercsCode.Powers;
using SpaceMercs.SpaceMercsCode.Relics;

namespace SpaceMercs.SpaceMercsCode.Relics;

public class Fireball() : SpaceMercsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ScorchPower>()
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner && player.PlayerCombatState != null && player.PlayerCombatState.TurnNumber == 1)
        {
            for (int i = 0; i < player.RunState.Players.Count; i++)
            {
                IEnumerable<Creature>? creatures = Owner.Creature.CombatState?.HittableEnemies;
                if (creatures == null)
                    return;
                Creature? randCreature = Owner.RunState.Rng.CombatTargets.NextItem(creatures);
                if (randCreature == null)
                    return;
                await PowerCmd.Apply<ScorchPower>(choiceContext, randCreature, 10, Owner.Creature, null);
            }
        }
    }
}