using BaseLib.Extensions;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.GameInfo.Objects;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class FierySplendor() : SpaceMercsCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (Owner.HasPower<CurePower>())
        {
            await PowerCmd.Apply<CurePower>(choiceContext, Owner.Creature, Owner.Creature.GetPower<CurePower>().Amount,
                Owner.Creature, this);
        }

        if (IsUpgraded)
        {
            foreach (Creature c in CombatState.HittableEnemies)
            {
                if (c.HasPower<ScorchPower>())
                {
                    await PowerCmd.Apply<ScorchPower>(choiceContext, c, c.GetPower<ScorchPower>().Amount, Owner.Creature, this);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {

    }
}