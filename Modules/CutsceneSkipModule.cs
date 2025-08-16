using HutongGames.PlayMaker.Actions;
using KorzUtils.Helper;
using UnityEngine;

namespace TheHuntIsOn.Modules;

internal class CutsceneSkipModule : Module
{
    #region Properties

    public override string MenuDescription => "Skips or speeds up certain cutscenes.";

    #endregion

    #region EventHandler

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed)
        {
            // Mothwing Cloak Cutscene Removal
            if (self.gameObject.name == "Dreamer Scene 1" && self.FsmName == "Control")
            {
                self.GetState("Init").AdjustTransitions("End");
            }

            // Dream Nail Entrance Speed-up
            if (self.gameObject.name == "Dreamer Plaque Inspect" && self.FsmName == "Conversation Control")
            {
                self.GetState("Hero Anim").RemoveActions<ActivateGameObject>();
                self.GetState("Hero Anim").AdjustTransitions("Map Msg?");
            }
            if (self.gameObject.name == "Dreamer Scene 2" && self.FsmName == "Control")
            {
                self.GetState("Take Control").AdjustTransitions("Fade Out");
                self.GetState("Fade Out").AdjustTransitions("Set Compass Point");
            }

            // Dream Nail Segment Seer Speed-up
            if (self.gameObject.name == "Witch Control" && self.FsmName == "Control")
            {
                self.GetState("First Pause").RemoveActions<Wait>();
                self.GetState("Witch Appear").RemoveActions<Wait>();
                self.GetState("Repeat Pause").RemoveActions<Wait>();
            }

            // Black Egg Temple Door Freeze Removal
            if (self.gameObject.name == "Final Boss Door" && self.FsmName == "Control")
            {
                self.GetState("Take Control").RemoveActions<SendMessage>();
                self.GetState("End").RemoveActions<SendMessage>();
            }

            // City of Tears Hornet Fountain Encounter
            if (self.gameObject.name == "Hornet Fountain Encounter" && self.FsmName == "Control")
            {
                self.GetState("Init").AdjustTransitions("End");
            }

            // Ancestral Mound Shaman Speed-up
            if (self.gameObject.name == "Shaman Meeting" && self.FsmName == "Conversation Control")
            {
                self.GetState("Turn?").AdjustTransitions("Spell Appear");
            }

            // Vengeful Spirit Collection Speed-up
            if (self.gameObject.name == "Knight Get Fireball" && self.FsmName == "Get Fireball")
            {
                self.GetState("Start").RemoveActions<Wait>();
                self.GetState("Get").RemoveActions<Wait>();
            }

            if (self.gameObject.name == "Knight Cutscene Animator" && self.FsmName == "Check Fall")
            {
                self.GetState("Black").RemoveActions<Wait>();
            }
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
