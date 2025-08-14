using Hkmp.Networking.Packet;

namespace TheHuntIsOn.Modules.PauseTimerModule;

internal static class IPacketExtensions
{
    internal static long? ReadOptionalLong(this IPacket self) => self.ReadBool() ? self.ReadLong() : null;

    internal static void WriteOptional(this IPacket self, long? value)
    {
        self.Write(value.HasValue);
        if (value.HasValue) self.Write(value.Value);
    }
}
