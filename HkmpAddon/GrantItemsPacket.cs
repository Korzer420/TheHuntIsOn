using Hkmp.Networking.Packet;

namespace TheHuntIsOn.HkmpAddon;

internal class GrantItemsPacket : IPacketData
{
    #region Properties

    public NetItem[] NetItems { get; set; }

    public bool IsReliable => true;
    public bool DropReliableDataIfNewerExists => false;

    #endregion

    #region Methods

    public void WriteData(IPacket packet)
    {
        var length = (byte)NetItems.Length;
        packet.Write(length);

        for (var i = 0; i < length; i++)
            packet.Write((byte)NetItems[i]);
    }

    public void ReadData(IPacket packet)
    {
        var length = packet.ReadByte();
        NetItems = new NetItem[length];
        for (var i = 0; i < length; i++)
            NetItems[i] = (NetItem)packet.ReadByte();
    } 

    #endregion
}