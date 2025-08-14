using Modding;

namespace TheHuntIsOn.Modules;

internal class StartingItemsModule : Module
{
    #region Properties

    public override string MenuDescription => "Provides starting items to players on fresh saves.";

    #endregion

    #region Eventhandler

    private void ModHooks_GrantStartingItems()
    {
        if (IsModuleUsed)
        {
            var pd = PlayerData.instance;

            pd.maxHealth = 4;
            pd.maxHealthBase = 4;

            pd.lurienDefeated = true;
            pd.monomonDefeated = true;
            pd.hegemolDefeated = true;

            pd.charmSlots = 5;
            pd.gotCharm_36 = true;
            pd.royalCharmState = 3;
            pd.equippedCharm_36 = true;

            pd.hasDreamNail = true;
            pd.dreamNailUpgraded = true;

            pd.hasLantern = true;
            pd.hasTramPass = true;
            pd.hasKingsBrand = true;

            pd.hasMap = true;
            //pd.mapAllRooms = true;
            pd.gladeDoorOpened = true;
            pd.troupeInTown = true;
            pd.geo = 9999;
        }
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        ModHooks.NewGameHook += ModHooks_GrantStartingItems;
    }

    internal override void Disable()
    {
        ModHooks.NewGameHook -= ModHooks_GrantStartingItems;
    }

    #endregion
}
