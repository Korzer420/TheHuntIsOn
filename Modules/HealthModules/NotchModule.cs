using KorzUtils.Helper;

namespace TheHuntIsOn.Modules.HealthModules;

internal class NotchModule : Module
{
    #region Properties

    public override string MenuDescription => "Blocks heal from picking up charm notches.";

    #endregion

    #region Eventhandler

    private void SetPlayerDataBool_OnEnter(On.HutongGames.PlayMaker.Actions.SetPlayerDataBool.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SetPlayerDataBool self)
    {
        if (self.IsCorrectContext("Shiny Control", null, "Notch"))
            HealthControl.BlockHeal = false;
        orig(self);
    }

    private void IncrementPlayerDataInt_OnEnter(On.HutongGames.PlayMaker.Actions.IncrementPlayerDataInt.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IncrementPlayerDataInt self)
    {
        if (IsModuleUsed && self.IsCorrectContext("Shiny Control", null, "Notch"))
            HealthControl.BlockHeal = true;
        orig(self);
    }

    #endregion

    #region Methods

    internal override void Enable() 
    { 
        On.HutongGames.PlayMaker.Actions.IncrementPlayerDataInt.OnEnter += IncrementPlayerDataInt_OnEnter;
        On.HutongGames.PlayMaker.Actions.SetPlayerDataBool.OnEnter += SetPlayerDataBool_OnEnter;
    }

    internal override void Disable()
    {
        On.HutongGames.PlayMaker.Actions.IncrementPlayerDataInt.OnEnter -= IncrementPlayerDataInt_OnEnter;
        On.HutongGames.PlayMaker.Actions.SetPlayerDataBool.OnEnter -= SetPlayerDataBool_OnEnter;
    }
    #endregion
}
