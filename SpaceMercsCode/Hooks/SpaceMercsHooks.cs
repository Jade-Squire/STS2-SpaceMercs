using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace SpaceMercs.SpaceMercsCode.Hooks;

public static class SpaceMercsHooks
{
    public static IEnumerable<AbstractModel> IterateSpaceMercsCombatHookListeners(ICombatState combatState)
    {
        if (!CombatManager.Instance.IsOverOrEnding || CombatManager.Instance.IsStarting)
        {
            foreach(AbstractModel iterateHookListener in combatState.IterateHookListeners())
                yield return iterateHookListener;
        }
    }

    public static async Task BeforeEnemyIgnited(ICombatState combatState, PlayerChoiceContext choiceContext, Creature ignitedCreature, Creature? applier,
        CardModel? cardSource)
    {
        foreach (AbstractModel model in IterateSpaceMercsCombatHookListeners(combatState))
        {
            if (model is IEnemyIgnited)
            {
                ((IEnemyIgnited)model).BeforeEnemyIgnited(choiceContext, ignitedCreature, applier, cardSource);
            }
            model.InvokeExecutionFinished();
        }
    }

    public static async Task AfterEnemyIgnited(ICombatState combatState, PlayerChoiceContext choiceContext, Creature ignitedCreature, Creature? applier,
        CardModel? cardSource)
    {
        foreach (AbstractModel model in IterateSpaceMercsCombatHookListeners(combatState))
        {
            if (model is IEnemyIgnited)
            {
                ((IEnemyIgnited)model).AfterEnemyIgnited(choiceContext, ignitedCreature, applier, cardSource);
            }

            model.InvokeExecutionFinished();
        }
    }

    public static async Task BeforeEnemyFrozen(ICombatState combatState, PlayerChoiceContext choiceContext,
        Creature frozenCreature, Creature? applier, CardModel? cardSource)
    {
        foreach (AbstractModel model in IterateSpaceMercsCombatHookListeners(combatState))
        {
            if (model is IEnemyFrozen)
            {
                ((IEnemyFrozen)model).BeforeEnemyFrozen(choiceContext, frozenCreature, applier, cardSource);
            }
            
            model.InvokeExecutionFinished();
        }
    }
    
    public static async Task AfterEnemyFrozen(ICombatState combatState, PlayerChoiceContext choiceContext, Creature frozenCreature, Creature? applier,
        CardModel? cardSource)
    {
        foreach (AbstractModel model in IterateSpaceMercsCombatHookListeners(combatState))
        {
            if (model is IEnemyFrozen)
            {
                ((IEnemyFrozen)model).AfterEnemyFrozen(choiceContext, frozenCreature, applier, cardSource);
            }

            model.InvokeExecutionFinished();
        }
    }
}