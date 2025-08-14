using Hkmp.Api.Server.Networking;
using Hkmp.Api.Server;

namespace TheHuntIsOn.Modules.PauseModule;

internal class ServerNetManager(ServerAddon addon, INetServer netServer)
{
    private readonly IServerAddonNetworkSender<ClientPacketId> _netSender = netServer.GetNetworkSender<ClientPacketId>(addon);

    public void BroadcastPacket(UpdatePauseStatePacket packet) => _netSender.BroadcastCollectionData(ClientPacketId.UpdatePauseState, packet);
    public void BroadcastPacket(UpdateCountdownsPacket packet) => _netSender.BroadcastCollectionData(ClientPacketId.UpdateCountdowns, packet);
    public void BroadcastPacket(SetRespawnTimerPacket packet) => _netSender.BroadcastCollectionData(ClientPacketId.SetRespawnTimer, packet);
}