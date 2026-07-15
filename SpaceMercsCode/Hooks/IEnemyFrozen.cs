using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SpaceMercs.SpaceMercsCode.Hooks;

public interface IEnemyFrozen
{
    public void BeforeEnemyFrozen(PlayerChoiceContext choiceContext, Creature frozenCreature, Creature? applier,
        CardModel? cardSource)
    {
        
    }

    public void AfterEnemyFrozen(PlayerChoiceContext choiceContext, Creature frozenCreature, Creature? applier,
        CardModel? cardSource)
    {
        
    }
}