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
    
    public static async Task BeforeEnemyJolted(ICombatState combatState, PlayerChoiceContext choiceContext,
        Creature joltedCreature, Creature? dealer, CardModel? cardSource, int joltUsed)
    {
        foreach (AbstractModel model in IterateSpaceMercsCombatHookListeners(combatState))
        {
            if (model is IEnemyJolted)
            {
                ((IEnemyJolted)model).BeforeEnemyJolted(choiceContext, joltedCreature, dealer, cardSource, joltUsed);
            }
            
            model.InvokeExecutionFinished();
        }
    }
    
    public static async Task AfterEnemyJolted(ICombatState combatState, PlayerChoiceContext choiceContext, Creature joltedCreature, Creature? dealer,
        CardModel? cardSource, int joltUsed)
    {
        foreach (AbstractModel model in IterateSpaceMercsCombatHookListeners(combatState))
        {
            if (model is IEnemyJolted && joltedCreature != null)
            {
                ((IEnemyJolted)model).AfterEnemyJolted(choiceContext, joltedCreature, dealer, cardSource, joltUsed);
            }

            model.InvokeExecutionFinished();
        }
    }

    public static bool ShouldLoseHungerAtTurnStart(ICombatState combatState, PlayerChoiceContext choiceContext, out AbstractModel? preventer)
    {
        foreach (AbstractModel model in IterateSpaceMercsCombatHookListeners(combatState))
        {
            if (model is IShouldLoseHunger)
            {
                if (!((IShouldLoseHunger)model).ShouldLoseHungerAtTurnStart())
                {
                    preventer = model;
                    return false;
                }
            }
        }
        preventer = null;
        return true;
    }
}