using Hkmp.Api.Client;
using Hkmp.Api.Server;
using System;

namespace TheHuntIsOn.Modules.PauseModule;

internal class PauseTimerModule : Module
{
    #region Properties

    public override string MenuDescription => "Enable server-wide pauses, timed unpauses, and respawn timers.";

    private PauseTimerClientAddon PauseTimerClientAddon;
    private PauseTimerServerAddon PauseTimerServerAddon;

    private bool AreAddonsLoaded;

    private readonly PauseController pauseController = new();
    private readonly CountdownsDisplayer countdownsDisplayer = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Initialize method used to register HKMP client and server addons after the module affections has been set.
    /// If the module should not affect any players, we do not register the addons. Otherwise, we do.
    /// This way if module affection is set to none and the game is restarted, players can connect to server that
    /// do not have the PauseTimer server addon.
    /// </summary>
    internal override void Initialize()
    {
        if (Affection == ModuleAffection.None)
        {
            AreAddonsLoaded = false;
            return;
        }

        PauseTimerClientAddon = new(this);
        PauseTimerServerAddon = new();
        ClientAddon.RegisterAddon(PauseTimerClientAddon);
        ServerAddon.RegisterAddon(PauseTimerServerAddon);

        AreAddonsLoaded = true;
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        if (!AreAddonsLoaded || !IsModuleUsed) return;

        pauseController.Enable();
        countdownsDisplayer.Enable();

        PauseTimerClientAddon.NetManager.SetRespawnTimerEvent += OnSetDeathTimer;
        PauseTimerClientAddon.NetManager.UpdateCountdownsEvent += OnUpdateCountdowns;
        PauseTimerClientAddon.NetManager.UpdatePauseStateEvent += OnUpdatePauseState;
    }

    internal override void Disable()
    {
        if (!AreAddonsLoaded || !IsModuleUsed) return;

        pauseController.Disable();
        countdownsDisplayer.Disable();

        PauseTimerClientAddon.NetManager.SetRespawnTimerEvent -= OnSetDeathTimer;
        PauseTimerClientAddon.NetManager.UpdateCountdownsEvent -= OnUpdateCountdowns;
        PauseTimerClientAddon.NetManager.UpdatePauseStateEvent -= OnUpdatePauseState;
    }

    internal void SetClientApi(IClientApi clientApi)
    {
        pauseController.SetClientApi(clientApi);
        countdownsDisplayer.SetClientApi(clientApi);
    }

    private void OnSetDeathTimer(SetRespawnTimerPacket packet) => TheHuntIsOn.LocalSaveData.RespawnTimerSeconds = packet.DeathTimer;

    private void OnUpdateCountdowns(UpdateCountdownsPacket packet) => TheHuntIsOn.LocalSaveData.UpdateCountdowns(DateTime.UtcNow, packet);

    private void OnUpdatePauseState(UpdatePauseStatePacket packet) => TheHuntIsOn.LocalSaveData.UpdatePauseState(packet);

    #endregion
}