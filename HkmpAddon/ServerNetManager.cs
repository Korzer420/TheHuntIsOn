using System;
using System.Collections.Generic;
using Hkmp.Api.Server;
using Hkmp.Api.Server.Networking;
using Hkmp.Networking.Packet;
using Hkmp.Networking.Packet.Data;

namespace TheHuntIsOn.HkmpAddon;

public class ServerNetManager
{
    public event Action<ushort, NetEvent> EventTriggeredEvent; 
    
    private readonly IServerAddonNetworkSender<ClientPacketId> _netSender;

    #region Constructors

    public ServerNetManager(ServerAddon addon, INetServer netServer)
    {
        _netSender = netServer.GetNetworkSender<ClientPacketId>(addon);

        var netReceiver = netServer.GetNetworkReceiver<ServerPacketId>(addon, InstantiatePacket);

        netReceiver.RegisterPacketHandler<EventTriggeredPacket>(
            ServerPacketId.EventTriggered,
            (id, packet) => EventTriggeredEvent?.Invoke(id, packet.NetEvent)
        );
    } 

    #endregion

    #region Methods

    public void SendGrantItems(List<NetItem> netItems)
    {
        _netSender.BroadcastCollectionData(ClientPacketId.GrantItems, new GrantItemsPacket
        {
            NetItems = [.. netItems]
        });
    }

    private static IPacketData InstantiatePacket(ServerPacketId packetId)
    {
        return packetId switch
        {
            ServerPacketId.EventTriggered => new PacketDataCollection<EventTriggeredPacket>(),
            _ => null,
        };
    } 

    #endregion
}