using Hkmp.Networking.Packet;
using System;
using TheHuntIsOn.Modules.PauseTimerModule;

namespace TheHuntIsOn.Modules.PauseModule;

// A representation of a pausable Countdown timer.
//
// A timer can be in three different states, depending on values:
//  - Actively ticking down to a deadline.
//  - Indefinitely frozen.
//  - Temporarily frozen, with a known un-freeze time.
public record Countdown
{
    // Scheduled time for the countdown to expire.
    public long FinishTimeTicks;
    // If set, always show this amount as the remaining time.
    public long? FrozenRemainder;
    // If set, override FrozenRemainder and start counting down again after this time.
    public long? UnfreezeTimeTicks;
    // Message to accompany the countdown.
    public string Message = "<untitled>";

    public bool IsFrozen(DateTime now) => FrozenRemainder.HasValue && (!UnfreezeTimeTicks.HasValue || UnfreezeTimeTicks.Value > now.Ticks);

    public bool IsCompleted(DateTime now) => !IsFrozen(now) && now.Ticks >= FinishTimeTicks;

    public bool GetDisplayTime(out float seconds)
    {
        var now = DateTime.UtcNow;
        if (IsFrozen(now) || !IsCompleted(now))
        {
            TimeSpan span = new(IsFrozen(now) ? FrozenRemainder.Value : (FinishTimeTicks - now.Ticks));
            seconds = (float)span.TotalSeconds;
            return true;
        }

        seconds = 0;
        return false;
    }

    public Countdown Pause(DateTime now)
    {
        if (IsCompleted(now)) return this;
        if (IsFrozen(now)) return this with { UnfreezeTimeTicks = null };

        return this with
        {
            FrozenRemainder = FinishTimeTicks - now.Ticks,
            UnfreezeTimeTicks = null
        };
    }

    public Countdown UnpauseAt(DateTime now, DateTime unpauseWhen)
    {
        if (IsCompleted(now)) return this;
        if (IsFrozen(now)) return this with
        {
            FinishTimeTicks = unpauseWhen.Ticks + FrozenRemainder.Value,
            UnfreezeTimeTicks = unpauseWhen.Ticks
        };

        var remainder = FinishTimeTicks - now.Ticks;
        return this with
        {
            FinishTimeTicks = unpauseWhen.Ticks + remainder,
            FrozenRemainder = remainder,
            UnfreezeTimeTicks = unpauseWhen.Ticks
        };
    }

    public void WriteData(IPacket packet)
    {
        packet.Write(FinishTimeTicks);
        packet.WriteOptional(FrozenRemainder);
        packet.WriteOptional(UnfreezeTimeTicks);
        packet.Write(Message);
    }

    public void ReadData(IPacket packet)
    {
        FinishTimeTicks = packet.ReadLong();
        FrozenRemainder = packet.ReadOptionalLong();
        UnfreezeTimeTicks = packet.ReadOptionalLong();
        Message = packet.ReadString();
    }
}
