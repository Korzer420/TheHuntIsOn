using Hkmp.Networking.Packet;

namespace TheHuntIsOn.Modules.PauseModule;

public class SetRespawnTimerPacket : IPacketData
{
    #region Properties

    public int DeathTimer;

    public bool IsReliable => true;

    public bool DropReliableDataIfNewerExists => true;

    #endregion

    #region Methods

    public void WriteData(IPacket packet) => packet.Write(DeathTimer);

    public void ReadData(IPacket packet) => DeathTimer = packet.ReadInt();

    #endregion
}
