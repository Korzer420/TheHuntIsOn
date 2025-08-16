using KorzUtils.Enums;
using KorzUtils.Helper;
using Modding;

namespace TheHuntIsOn.Modules;

internal class CharmNerfModule : Module
{
    #region Properties

    public override string MenuDescription => "Increases the charm notch cost of powerful PvP charms.";

    #endregion

    #region Eventhandler

    private bool ModHooks_SetPlayerBoolHook(string name, bool orig)
    {
        if (IsModuleUsed && name.StartsWith("gotCharm_") && orig)
        {
            if (int.TryParse(name.Substring("gotCharm_".Length), out int charmNumber))
                CoroutineHelper.WaitFrames(x =>
                {
                    switch (charmNumber)
                    {
                        case 05: // Baldur Shell
                            CharmHelper.SetCharmCost(x, 3);
                            break;
                        case 17: // Spore Shroom
                            CharmHelper.SetCharmCost(x, 2);
                            break;
                        case 23: // Heart
                            CharmHelper.SetCharmCost(x, 3);
                            break;
                        case 26: // Nailmaster's Glory
                            CharmHelper.SetCharmCost(x, 2);
                            break;
                        default:
                            break;
                    }
                }, (CharmRef)charmNumber, true, 3);
        }
        return orig;
    }

    #endregion

    #region Methods

    internal override void Enable() => ModHooks.SetPlayerBoolHook += ModHooks_SetPlayerBoolHook;

    internal override void Disable() => ModHooks.SetPlayerBoolHook -= ModHooks_SetPlayerBoolHook;
	#endregion
}
