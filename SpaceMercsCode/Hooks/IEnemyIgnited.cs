using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SpaceMercs.SpaceMercsCode.Hooks;

public interface IEnemyIgnited
{
    public void BeforeEnemyIgnited(PlayerChoiceContext choiceContext, Creature ignitedCreature, Creature? applier,
        CardModel? cardSource)
    {
    }

    public void AfterEnemyIgnited(PlayerChoiceContext choiceContext, Creature ignitedCreature, Creature? applier,
        CardModel? cardSource)
    {
    }
}