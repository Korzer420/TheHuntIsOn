using KorzUtils.Data;
using KorzUtils.Helper;

namespace TheHuntIsOn.Modules;

internal class StagModule : Module
{
    #region Properties

    public override string MenuDescription => "Disable Stags.";

    #endregion

    #region Eventhandler

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed)
        {
            if (self.gameObject.name == "Stag" && (self.FsmName == "Stag Control" || self.FsmName == "npc_control"))
            {
                self.AddState("Sturdy", () => GameHelper.DisplayMessage("They don't seem to listen..."),
                    FsmTransitionData.FromTargetState("Cancel Frame").WithEventName("FINISHED"));
                self.GetState("Can Talk?").AdjustTransitions("Sturdy");
            }
        }

        orig(self);
    }

    #endregion

    #region Methods

    internal override void Enable() => On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
    
    internal override void Disable() => On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
    
    #endregion
}
