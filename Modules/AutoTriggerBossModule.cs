using KorzUtils.Helper;

namespace TheHuntIsOn.Modules;

internal class AutoTriggerBossModule : Module
{
    #region Properties

    public override string MenuDescription => "Starts certain boss (uncleared) encounters automatically when in range.";

    #endregion

    #region EventHandler

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed)
        {
            if (self.gameObject.name == "Ghost Warrior NPC" && self.FsmName == "Appear")
                self.GetState("Close").AddActions(() => self.gameObject.LocateMyFSM("Conversation Control").SendEvent("DREAMNAIL"));
            else if (self.gameObject.name == "Challenge Prompt" && self.FsmName == "Challenge Start"
                && self.gameObject.scene.name == "Fungus2_15_boss")
                self.GetState("Turns?").AdjustTransitions("Take Control");
        }

        orig(self);
    }

    #endregion

    #region Methods

    internal override void Disable()
    {
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
    }

    internal override void Enable()
    {
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
    }

    #endregion
}
