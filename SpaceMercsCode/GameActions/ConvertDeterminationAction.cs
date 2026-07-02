using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpaceMercs.SpaceMercsCode.Commands;

namespace SpaceMercs.SpaceMercsCode.GameActions;

public class ConvertDeterminationAction : GameAction
{
    public readonly Player _player;
    public override ulong OwnerId => _player.NetId;

    public ConvertDeterminationAction(Player player)
    {
        _player = player;
    }
    
    protected override Task ExecuteAction()
    {
        CosmopaladinPlayerCmd.LoseDetermination(2, _player);
        PlayerCmd.GainEnergy(1, _player);
        return Task.CompletedTask;
    }

    public override INetAction ToNetAction()
    {
        return new NetConvertDeterminationAction();
    }

    public override GameActionType ActionType => GameActionType.Combat;
}