using Hkmp.Api.Command.Server;
using Hkmp.Api.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheHuntIsOn.Modules.PauseModule;

internal class PauseTimerCommand(IServerApi serverApi, ServerNetManager netManager) : IServerCommand
{
    public string Trigger => "/pausetimer";

    public string[] Aliases => ["/pt"];

    public bool AuthorizedOnly => true;

    internal HuntLocalSaveData ServerState = new();

    internal ServerNetManager NetManager => netManager;

    internal void BroadcastMessage(string message) => serverApi.ServerManager.BroadcastMessage(message);

    private static readonly List<PauseTimerSubcommand> subcommands =
    [
        new PauseSubcommand(),
        new UnpauseSubcommand(),
        new CountdownSubcommand(),
        new ClearCountdownsSubcommand(),
        new SetRespawnTimerSubcommand()
    ];

    private static string AllSubcommands() => string.Join("|", [.. subcommands.Select(s => s.Name()).OrderBy(s => s)]);

    private static bool TryGetSubcommand(string name, out PauseTimerSubcommand subcommand)
    {
        foreach (var item in subcommands)
        {
            if (item.Name() == name || item.Aliases().Any(a => a == name))
            {
                subcommand = item;
                return true;
            }
        }

        subcommand = null;
        return false;
    }

    internal static bool MinArguments(ICommandSender commandSender, string[] arguments, int min)
    {
        if (arguments.Length >= min) return true;

        commandSender.SendMessage("Missing arguments.");
        return false;
    }

    internal static bool MaxArguments(ICommandSender commandSender, string[] arguments, int max)
    {
        if (arguments.Length <= max) return true;

        commandSender.SendMessage("Too many arguments.");
        return false;
    }

    internal static bool ParseInt(ICommandSender commandSender, string arg, out int value)
    {
        if (int.TryParse(arg, out value) && value >= 0) return true;
        
        commandSender.SendMessage($"Invalid integer '{arg}'");
        return false;
    }

    private void UpdateCountdowns(DateTime now, Func<Countdown, Countdown> map)
    {
        UpdateCountdownsPacket packet = new() { Countdowns = [.. ServerState.GlobalCountdowns.Select(map)] };
        ServerState.UpdateCountdowns(now, packet);
        netManager.BroadcastPacket(packet);
    }

    internal void PauseCountdowns(DateTime now) => UpdateCountdowns(now, c => c.Pause(now));

    internal void UnpauseCountdowns(DateTime now, DateTime unpauseWhen) => UpdateCountdowns(now, c => c.UnpauseAt(now, unpauseWhen));

    public void Execute(ICommandSender commandSender, string[] arguments)
    {
        if (arguments.Length <= 1)
        {
            commandSender.SendMessage($"Usage: '/pt <{AllSubcommands()}>'");
            commandSender.SendMessage("Use '/pt help <command>' for details");
            return;
        }

        PauseTimerSubcommand subcommand;
        string name = arguments[1].ToLower();
        if (name == "help")
        {
            if (arguments.Length == 2) commandSender.SendMessage($"Usage: '/pt help <{AllSubcommands()}>'");
            else if (TryGetSubcommand(arguments[2].ToLower(), out subcommand))
            {
                commandSender.SendMessage($"Usage: {subcommand.Usage()}");

                List<string> aliases = [.. subcommand.Aliases()];
                if (aliases.Count > 0)
                {
                    aliases.Sort();
                    commandSender.SendMessage($"Aliases: {string.Join("|", aliases)}");
                }
            }
            else
            {
                commandSender.SendMessage($"Unrecognized command '{name}'.");
                commandSender.SendMessage($"Usage: '/pt help <{AllSubcommands()}>'");
            }
            return;
        }

        if (!TryGetSubcommand(name, out subcommand))
        {
            commandSender.SendMessage($"Unrecognized command '{name}'.");
            commandSender.SendMessage($"Usage: '/pt <{AllSubcommands()}>'");
            return;
        }

        if (!subcommand.Execute(this, commandSender, [.. arguments.Skip(2)])) commandSender.SendMessage($"Usage: {subcommand.Usage()}");
    }
}

internal abstract class PauseTimerSubcommand
{
    public abstract string Name();

    public virtual IEnumerable<string> Aliases() => [];

    public abstract string Usage();

    public abstract bool Execute(PauseTimerCommand parent, ICommandSender send, string[] arguments);
}

internal class PauseSubcommand : PauseTimerSubcommand
{
    public override string Name() => "pause";

    public override string Usage() => "'/pt pause [X]': Pause the game for all players. If X is specified, unpause after X seconds.";

    public override bool Execute(PauseTimerCommand parent, ICommandSender commandSender, string[] arguments)
    {
        if (!PauseTimerCommand.MaxArguments(commandSender, arguments, 1)) return false;

        UpdatePauseStatePacket packet = new() { ServerPaused = true, UnpauseTimeTicks = long.MaxValue };
        var now = DateTime.UtcNow;
        int seconds = 0;
        if (arguments.Length == 1)
        {
            if (!PauseTimerCommand.ParseInt(commandSender, arguments[0], out seconds)) return false;

            var unpauseAt = now.AddSeconds(seconds);
            packet.UnpauseTimeTicks = unpauseAt.Ticks;
            parent.PauseCountdowns(now);
            parent.UnpauseCountdowns(now, unpauseAt);
        }
        else parent.PauseCountdowns(now);

        parent.ServerState.UpdatePauseState(packet);
        parent.NetManager.BroadcastPacket(packet);
        commandSender.SendMessage(seconds == 0 ? "Paused server." : $"Paused server for {seconds} seconds.");
        parent.BroadcastMessage(seconds == 0 ? "Server paused." : $"Server paused for {seconds} seconds.");
        return true;
    }
}

internal class UnpauseSubcommand : PauseTimerSubcommand
{
    public override string Name() => "unpause";

    public override string Usage() => "'/pt unpause [X]': Unpause the game for all players. If X is specified, unpause after X seconds.";

    public override bool Execute(PauseTimerCommand parent, ICommandSender commandSender, string[] arguments)
    {
        if (!PauseTimerCommand.MaxArguments(commandSender, arguments, 1)) return false;

        var isPaused = parent.ServerState.ServerPaused;
        if (!isPaused)
        {
            commandSender.SendMessage("Server is already unpaused.");
            return true;
        }

        var now = DateTime.UtcNow;
        UpdatePauseStatePacket packet = new();
        int seconds = 0;
        if (arguments.Length == 1)
        {
            if (!PauseTimerCommand.ParseInt(commandSender, arguments[0], out seconds)) return false;

            var unpauseAt = now.AddSeconds(seconds);
            packet.ServerPaused = true;
            packet.UnpauseTimeTicks = unpauseAt.Ticks;
            parent.UnpauseCountdowns(now, unpauseAt);
        }
        else
        {
            packet.ServerPaused = false;
            packet.UnpauseTimeTicks = 0;
            parent.UnpauseCountdowns(now, now);
        }

        parent.ServerState.UpdatePauseState(packet);
        parent.NetManager.BroadcastPacket(packet);
        commandSender.SendMessage(seconds == 0 ? "Unpaused server." : $"Scheduled unpause in {seconds} seconds.");
        parent.BroadcastMessage(seconds == 0 ? "Server unpaused." : $"Server unpausing in {seconds} seconds.");
        return true;
    }
}

internal class CountdownSubcommand : PauseTimerSubcommand
{
    public override string Name() => "countdown";

    public override string Usage() => "'/pt countdown X [msg...]': Start a countdown for all players on the server lasting X seconds, with an optional message attached.";

    public override bool Execute(PauseTimerCommand parent, ICommandSender commandSender, string[] arguments)
    {
        if (!PauseTimerCommand.MinArguments(commandSender, arguments, 1)) return false;

        if (!PauseTimerCommand.ParseInt(commandSender, arguments[0], out var seconds)) return false;

        var now = DateTime.UtcNow;
        Countdown countdown = new() { FinishTimeTicks = now.AddSeconds(seconds).Ticks };

        // Respect any active pauses or timed unpauses.
        if (parent.ServerState.IsServerPaused(out var remaining))
        {
            if (remaining.HasValue) countdown = countdown.UnpauseAt(now, now.AddSeconds(remaining.Value));
            else countdown = countdown.Pause(now);
        }

        if (arguments.Length > 1)
        {
            countdown.Message = string.Join(" ", arguments.Skip(1));
            if (countdown.Message.Length > UpdateCountdownsPacket.MaxMessageLength)
            {
                commandSender.SendMessage("Countdown message is too long.");
                return false;
            }
        }
            
        UpdateCountdownsPacket packet = new() { Countdowns = [.. parent.ServerState.GlobalCountdowns.Concat([countdown])] };
        if (packet.Countdowns.Count > UpdateCountdownsPacket.MaxCountdowns)
        {
            commandSender.SendMessage("Too many countdowns. Try '/pt clearcountdowns'.");
            return true;
        }

        parent.ServerState.UpdateCountdowns(now, packet);
        parent.NetManager.BroadcastPacket(packet);
        commandSender.SendMessage($"Broadcasted new {seconds} second countdown.");
        return true;
    }
}

internal class ClearCountdownsSubcommand : PauseTimerSubcommand
{
    public override string Name() => "clearcountdowns";

    public override string Usage() => "'/pt clearcountdowns': clear all outstanding countdowns";

    public override bool Execute(PauseTimerCommand parent, ICommandSender commandSender, string[] arguments)
    {
        if (!PauseTimerCommand.MaxArguments(commandSender, arguments, 0)) return false;

        UpdateCountdownsPacket packet = new();
        parent.ServerState.UpdateCountdowns(DateTime.UtcNow, packet);
        parent.NetManager.BroadcastPacket(packet);
        commandSender.SendMessage("Cleared all active countdowns.");
        return true;
    }
}

internal class SetRespawnTimerSubcommand : PauseTimerSubcommand
{
    public override string Name() => "respawntimer";

    public override IEnumerable<string> Aliases() => ["deathtimer"];

    public override string Usage() => "'/pt respawntimer [X]': get the current respawn delay on death, or else set it";

    public override bool Execute(PauseTimerCommand parent, ICommandSender commandSender, string[] arguments)
    {
        if (!PauseTimerCommand.MaxArguments(commandSender, arguments, 1)) return false;

        if (arguments.Length == 0)
        {
            var time = parent.ServerState.RespawnTimerSeconds;
            if (time == 0) commandSender.SendMessage("Respawn timer is not set.");
            else commandSender.SendMessage($"Respawn timer is set to {time} seconds.");
            return true;
        }

        if (!PauseTimerCommand.ParseInt(commandSender, arguments[0], out int seconds)) return false;

        parent.ServerState.RespawnTimerSeconds = seconds;
        parent.NetManager.BroadcastPacket(new SetRespawnTimerPacket() { DeathTimer = seconds });
        parent.BroadcastMessage($"Respawn timer updated to {seconds} seconds.");
        return true;
    }
}
