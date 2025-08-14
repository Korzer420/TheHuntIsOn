using Hkmp.Networking.Packet;

namespace TheHuntIsOn.Modules.PauseModule;

public class UpdatePauseStatePacket : IPacketData
{
    public bool ServerPaused;

    public long UnpauseTimeTicks;

    public bool IsReliable => true;

    public bool DropReliableDataIfNewerExists => true;

    public void ReadData(IPacket packet)
    {
        ServerPaused = packet.ReadBool();
        UnpauseTimeTicks = packet.ReadLong();
    }

    public void WriteData(IPacket packet)
    {
        packet.Write(ServerPaused);
        packet.Write(UnpauseTimeTicks);
    }
}
