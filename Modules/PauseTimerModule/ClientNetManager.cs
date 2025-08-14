using System;
using Hkmp.Api.Client;
using Hkmp.Api.Client.Networking;
using Hkmp.Networking.Packet;
using Hkmp.Networking.Packet.Data;

namespace TheHuntIsOn.Modules.PauseModule;

public class ClientNetManager
{
    public event Action<SetRespawnTimerPacket> SetRespawnTimerEvent;
    public event Action<UpdateCountdownsPacket> UpdateCountdownsEvent;
    public event Action<UpdatePauseStatePacket> UpdatePauseStateEvent;

    public ClientNetManager(ClientAddon addon, INetClient netClient)
    {
        var netReceiver = netClient.GetNetworkReceiver<ClientPacketId>(addon, InstantiatePacket);

        netReceiver.RegisterPacketHandler<SetRespawnTimerPacket>(ClientPacketId.SetRespawnTimer, packet => SetRespawnTimerEvent?.Invoke(packet));
        netReceiver.RegisterPacketHandler<UpdateCountdownsPacket>(ClientPacketId.UpdateCountdowns, packet => UpdateCountdownsEvent?.Invoke(packet));
        netReceiver.RegisterPacketHandler<UpdatePauseStatePacket>(ClientPacketId.UpdatePauseState, packet => UpdatePauseStateEvent?.Invoke(packet));
    }

    private static IPacketData InstantiatePacket(ClientPacketId packetId)
        => packetId switch
        {
            ClientPacketId.SetRespawnTimer => new PacketDataCollection<SetRespawnTimerPacket>(),
            ClientPacketId.UpdateCountdowns => new PacketDataCollection<UpdateCountdownsPacket>(),
            ClientPacketId.UpdatePauseState => new PacketDataCollection<UpdatePauseStatePacket>(),
            _ => null,
        };
}