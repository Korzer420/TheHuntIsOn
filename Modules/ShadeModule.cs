using KorzUtils.Helper;
using Modding;
using UnityEngine;

namespace TheHuntIsOn.Modules;

internal class ShadeModule : Module
{
    #region Properties

    public override string MenuDescription => "Blocks the shade from appearing (and their effect).";

    #endregion

    #region Eventhandler

    private void PreventShadePenality()
    {
        if (!IsModuleUsed)
            return;
        HeroController.instance.AddGeoQuietly(PlayerData.instance.GetInt(nameof(PlayerData.geoPool)));
        PlayerData.instance.EndSoulLimiter();
        PlayerData.instance.SetInt(nameof(PlayerData.geoPool), 0);
    }

    private bool ModHooks_GetPlayerBoolHook(string name, bool orig)
    {
        if (IsModuleUsed && name == "soulLimited")
            return false;
        return orig;
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        ModHooks.AfterPlayerDeadHook += PreventShadePenality;
        ModHooks.GetPlayerBoolHook += ModHooks_GetPlayerBoolHook;
    }

    internal override void Disable()
    {
        ModHooks.AfterPlayerDeadHook -= PreventShadePenality;
        ModHooks.GetPlayerBoolHook -= ModHooks_GetPlayerBoolHook;
    }

    #endregion
}
