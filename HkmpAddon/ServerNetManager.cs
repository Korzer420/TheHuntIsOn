using System;
using System.Collections.Generic;
using Hkmp.Api.Server;
using Hkmp.Api.Server.Networking;
using Hkmp.Networking.Packet;
using Hkmp.Networking.Packet.Data;

namespace TheHuntIsOn.HkmpAddon;

public class ServerNetManager
{
    public event Action<ushort, NetItem> ItemObtainedEvent; 
    
    private readonly IServerAddonNetworkSender<ClientPacketId> _netSender;
    
    public ServerNetManager(ServerAddon addon, INetServer netServer)
    {
        _netSender = netServer.GetNetworkSender<ClientPacketId>(addon);

        var netReceiver = netServer.GetNetworkReceiver<ServerPacketId>(addon, InstantiatePacket);
        
        netReceiver.RegisterPacketHandler<ItemObtainedPacket>(
            ServerPacketId.ItemObtained, 
            (id, packet) => ItemObtainedEvent?.Invoke(id, packet.NetItem)
        );
    }

    public void SendGrantItems(List<NetItem> netItems)
    {
        _netSender.BroadcastCollectionData(ClientPacketId.GrantItems, new GrantItemsPacket
        {
            NetItems = netItems.ToArray()
        });
    }

    private static IPacketData InstantiatePacket(ServerPacketId packetId)
    {
        switch (packetId)
        {
            case ServerPacketId.ItemObtained:
                return new PacketDataCollection<ItemObtainedPacket>();
        }

        return null;
    }
}