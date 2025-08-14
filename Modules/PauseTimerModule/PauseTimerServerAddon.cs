using Hkmp.Api.Server;

namespace TheHuntIsOn.Modules.PauseModule;

internal class PauseTimerServerAddon : ServerAddon
{
    protected override string Name => AddonIdentifier.Name;

    protected override string Version => AddonIdentifier.Version;

    public override bool NeedsNetwork => true;

    public override void Initialize(IServerApi serverApi) => serverApi.CommandManager.RegisterCommand(new PauseTimerCommand(serverApi, new ServerNetManager(this, serverApi.NetServer)));
}
