using Hkmp.Api.Client;
using System.Collections;

namespace TheHuntIsOn.Modules.PauseModule;

internal class PauseController
{
    private float baseGameTimescale;
    private float hkmpTimescale;

    private bool enabled;
    private IClientApi clientApi;

    private void HookClientApi()
    {
        var mgr = clientApi.ClientManager;
        mgr.ConnectEvent += FixTimeScale;
        mgr.DisconnectEvent += FixTimeScale;
        mgr.OnSetTimeScale += OnHKMPSetTimeScale;
    }

    private void UnhookClientApi()
    {
        var mgr = clientApi.ClientManager;
        mgr.ConnectEvent -= FixTimeScale;
        mgr.DisconnectEvent -= FixTimeScale;
        mgr.OnSetTimeScale -= OnHKMPSetTimeScale;
    }

    internal void Enable()
    {
        enabled = true;
        baseGameTimescale = hkmpTimescale = TimeController.GenericTimeScale;

        On.GameManager.SetTimeScale_float += OnGMSetTimeScaleF;
        On.GameManager.SetTimeScale_float_float += OnGMSetTimeScaleFF;
        On.GameManager.Update += OnGMUpdate;
        if (clientApi != null) HookClientApi();
    }

    internal void SetClientApi(IClientApi clientApi)
    {
        if (this.clientApi != null) throw new System.ArgumentException("Cannot set clientApi twice");

        this.clientApi = clientApi;
        if (enabled) HookClientApi();
    }

    internal void Disable()
    {
        enabled = false;

        On.GameManager.SetTimeScale_float -= OnGMSetTimeScaleF;
        On.GameManager.SetTimeScale_float_float -= OnGMSetTimeScaleFF;
        On.GameManager.Update -= OnGMUpdate;

        if (clientApi != null) UnhookClientApi();
        clientApi = null;
    }

    private void OnGMSetTimeScaleF(On.GameManager.orig_SetTimeScale_float orig, GameManager self, float timeScale)
    {
        TimeController.GenericTimeScale = baseGameTimescale;
        orig(self, timeScale);
        baseGameTimescale = timeScale;
        hkmpTimescale = timeScale;
        FixTimeScale();
    }

    private IEnumerator OnGMSetTimeScaleFF(On.GameManager.orig_SetTimeScale_float_float orig, GameManager self, float timeScale, float duration)
    {
        var original = orig(self, timeScale, duration);

        IEnumerator Altered()
        {
            while (true)
            {
                TimeController.GenericTimeScale = baseGameTimescale;
                if (original.MoveNext())
                {
                    baseGameTimescale = TimeController.GenericTimeScale;
                    hkmpTimescale = TimeController.GenericTimeScale;
                    FixTimeScale();

                    yield return original.Current;
                }
                else break;
            }
        }
        return Altered();
    }

    private void OnGMUpdate(On.GameManager.orig_Update orig, GameManager self)
    {
        FixTimeScale();
        orig(self);
    }

    private void OnHKMPSetTimeScale(float timeScale)
    {
        hkmpTimescale = timeScale;
        FixTimeScale();
    }

    internal bool IsServerPaused() => enabled && clientApi != null && clientApi.NetClient.IsConnected && TheHuntIsOn.LocalSaveData.IsServerPaused(out _);

    internal static bool IsGameplayPausable() => GameManager.instance.gameState == GlobalEnums.GameState.PAUSED || (GameManager.instance.gameState == GlobalEnums.GameState.PLAYING && HeroController.instance.acceptingInput);

    private void FixTimeScale() => TimeController.GenericTimeScale = (IsServerPaused() && IsGameplayPausable()) ? 0f : hkmpTimescale;
}
