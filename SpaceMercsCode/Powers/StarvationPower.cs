using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards.Rare;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Powers;

public class StarvationPower() : SpaceMercsPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    private int CurrentIncreaseAmt = 0;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("IncreaseAmt", CurrentIncreaseAmt)
    ];

    public override async Task AfterSideTurnStartLate(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != Owner.Side)
        {
            return;
        }
        await PowerCmd.Apply<HungerPower>(new ThrowingPlayerChoiceContext(), Owner, Amount, Owner, null);
        await PowerCmd.Apply<StarvationPower>(new ThrowingPlayerChoiceContext(), Owner, DynamicVars["IncreaseAmt"].BaseValue, Owner, null);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
            return Task.CompletedTask;
        foreach (var enemy in CombatState.HittableEnemies)
        {
            CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemy,
                new DamageVar(Owner.Player?.PlayerCombatState?.Energy ?? 0, ValueProp.Unpowered), Owner, null);
        }
        return Task.CompletedTask;
    }

    public void AddIncreaseAmt(int amount)
    {
        DynamicVars["IncreaseAmt"].BaseValue += amount;
    }

}