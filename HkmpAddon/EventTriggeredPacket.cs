using Hkmp.Networking.Packet;

namespace TheHuntIsOn.HkmpAddon;

internal class EventTriggeredPacket : IPacketData
{
    #region Properties

    public NetEvent NetEvent { get; set; }

    public bool IsReliable => true;

    public bool DropReliableDataIfNewerExists => false;

    #endregion

    #region Methods

    public void WriteData(IPacket packet) => packet.Write((byte)NetEvent);

    public void ReadData(IPacket packet) => NetEvent = (NetEvent)packet.ReadByte(); 

    #endregion
}