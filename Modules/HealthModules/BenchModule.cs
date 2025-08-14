using HutongGames.PlayMaker.Actions;
using KorzUtils.Helper;

namespace TheHuntIsOn.Modules.HealthModules;

internal class BenchModule : Module
{
    #region Properties

    public override string MenuDescription => "Blocks heal from benches.";

    #endregion

    #region Eventhandler

    private void SetPlayerDataBool_OnEnter(On.HutongGames.PlayMaker.Actions.SetPlayerDataBool.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SetPlayerDataBool self)
    {
        if (IsModuleUsed && self.IsCorrectContext("Bench Control", null, "Start Rest"))
            HealthControl.BlockHeal = true;
        else if (IsModuleUsed && self.IsCorrectContext("Bench Control", null, "Regain Control"))
        {
            if (HealthControl.BlockHeal)
            {
                int currentSoul = PlayerData.instance.GetInt(nameof(PlayerData.MPCharge));
                GameCameras.instance.hudCanvas.gameObject.SetActive(false);
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);

                // The soul is depleted on reappearing so we restore that.
                CoroutineHelper.WaitFrames(HeroController.instance.AddMPCharge, currentSoul, true);
                self.State.GetFirstAction<Wait>().time.Value = 3.5f;
            }
            else
                self.State.GetFirstAction<Wait>().time.Value = 0.5f;
            HealthControl.BlockHeal = false;
        }
        orig(self);
    }

    #endregion

    #region Methods

    internal override void Enable() => On.HutongGames.PlayMaker.Actions.SetPlayerDataBool.OnEnter += SetPlayerDataBool_OnEnter;

    internal override void Disable() => On.HutongGames.PlayMaker.Actions.SetPlayerDataBool.OnEnter -= SetPlayerDataBool_OnEnter;

    #endregion
}
