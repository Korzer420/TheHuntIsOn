using System;
using Hkmp.Api.Client;
using Hkmp.Api.Client.Networking;
using Hkmp.Networking.Packet;
using Hkmp.Networking.Packet.Data;

namespace TheHuntIsOn.HkmpAddon;

public class ClientNetManager
{
    public event Action<NetItem[]> GrantItemsEvent;

    #region Members

    private readonly INetClient _netClient;

    private readonly IClientAddonNetworkSender<ServerPacketId> _netSender; 

    #endregion

    #region Constructors

    public ClientNetManager(ClientAddon addon, INetClient netClient)
    {
        _netClient = netClient;
        _netSender = netClient.GetNetworkSender<ServerPacketId>(addon);

        var netReceiver = netClient.GetNetworkReceiver<ClientPacketId>(addon, InstantiatePacket);

        netReceiver.RegisterPacketHandler<GrantItemsPacket>(
            ClientPacketId.GrantItems,
            packetData => GrantItemsEvent?.Invoke(packetData.NetItems));
    }

    #endregion

    #region Methods

    public void SendEvent(NetEvent netEvent)
    {
        if (!_netClient.IsConnected)
            return;

        _netSender.SendCollectionData(ServerPacketId.EventTriggered, new EventTriggeredPacket
        {
            NetEvent = netEvent
        });
    }

    private static IPacketData InstantiatePacket(ClientPacketId packetId)
        => packetId switch
        {
            ClientPacketId.GrantItems => new PacketDataCollection<GrantItemsPacket>(),
            _ => null,
        }; 

    #endregion
}