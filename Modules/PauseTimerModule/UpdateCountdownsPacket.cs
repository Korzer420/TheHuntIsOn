using Hkmp.Networking.Packet;
using System.Collections.Generic;

namespace TheHuntIsOn.Modules.PauseModule;

public class UpdateCountdownsPacket : IPacketData
{
    public const byte MaxMessageLength = byte.MaxValue;

    public const int MaxCountdowns = 10;

    #region Properties

    public List<Countdown> Countdowns = [];

    public bool IsReliable => true;

    public bool DropReliableDataIfNewerExists => true;

    #endregion

    #region Methods

    public void WriteData(IPacket packet)
    {
        packet.Write(Countdowns.Count);
        Countdowns.ForEach(c => c.WriteData(packet));
    }

    public void ReadData(IPacket packet)
    {
        int size = packet.ReadInt();
        Countdowns.Clear();
        for (int i = 0; i < size; i++)
        {
            Countdown countdown = new();
            countdown.ReadData(packet);
            Countdowns.Add(countdown);
        }
    }

    #endregion
}
