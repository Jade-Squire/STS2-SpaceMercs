using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace SpaceMercs.SpaceMercsCode.GameActions;

public struct NetConvertDeterminationAction : INetAction, IPacketSerializable
{
    
    public void Serialize(PacketWriter writer)
    {
        
    }

    public void Deserialize(PacketReader reader)
    {
        
    }

    public GameAction ToGameAction(Player player)
    {
        return new ConvertDeterminationAction(player);
    }
}