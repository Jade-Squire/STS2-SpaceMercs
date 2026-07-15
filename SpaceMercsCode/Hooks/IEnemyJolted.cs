using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SpaceMercs.SpaceMercsCode.Hooks;

public interface IEnemyJolted
{
    public void BeforeEnemyJolted(PlayerChoiceContext choiceContext, Creature joltedCreature, Creature? dealer,
        CardModel? cardSource, int joltUsed)
    {
        
    }

    public void AfterEnemyJolted(PlayerChoiceContext choiceContext, Creature joltedCreature, Creature? dealer,
        CardModel? cardSource, int joltUsed)
    {
        
    }
}