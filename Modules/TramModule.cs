using KorzUtils.Data;
using KorzUtils.Helper;

namespace TheHuntIsOn.Modules;

internal class TramModule : Module
{
    #region Properties

    public override string MenuDescription => "Disable trams.";

    #endregion

    #region Eventhandler

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed)
        {
            if (self.gameObject.name == "Tram Call Box" && self.FsmName == "Conversation Control")
            {
                self.AddState("Malfunction", () => GameHelper.DisplayMessage("Doesn't seem to work."),
                    FsmTransitionData.FromTargetState("End").WithEventName("FINISHED"));
                self.GetState("Got Pass?").AdjustTransitions("Malfunction");
            }
            else if (self.gameObject.name == "Door Inspect" && self.FsmName == "Tram Door")
            {
                self.AddState("Malfunction", () => GameHelper.DisplayMessage("Doesn't seem to work."),
                    FsmTransitionData.FromTargetState("Box Down").WithEventName("FINISHED"));
                self.GetState("Check Pass").AdjustTransitions("Malfunction");
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
