using HutongGames.PlayMaker.Actions;
using KorzUtils.Helper;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheHuntIsOn.Modules;

internal class DreamEntranceModule : Module
{
    #region Properties

    public override string MenuDescription => "Places dream boss entrances outside the arena.";

    #endregion

    #region Eventhandler

    private void DeactivateIfPlayerdataTrue_OnEnable(On.DeactivateIfPlayerdataTrue.orig_OnEnable orig, DeactivateIfPlayerdataTrue self)
    {
        if (IsModuleUsed && self.gameObject.name == "Dung Defender_Sleep")
            return;
        orig(self);
    }

    private void DeactivateIfPlayerdataFalse_OnEnable(On.DeactivateIfPlayerdataFalse.orig_OnEnable orig, DeactivateIfPlayerdataFalse self)
    {
        if (IsModuleUsed && self.gameObject.name == "Dung Defender_Sleep")
            return;
        orig(self);
    }

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed)
        {
            if (self.gameObject.name == "Mage Lord Remains" && self.FsmName == "Control")
            {
                // Create clone for both sides of the arena.
                self.transform.position = new Vector3(4.25f, 18.4f, self.transform.position.z);
                GameObject.Instantiate(self.gameObject).transform.position = new Vector3(41.17f, 29.41f, self.transform.position.z);

            }
            else if (self.gameObject.name == "IK Remains")
            {
                if (self.FsmName == "Control")
                    self.transform.position = new Vector3(44.1f, 28.1f, self.transform.position.z);
                else
                    Component.Destroy(self);
            }
            else if (self.gameObject.name == "Basement Open")
            {
                self.gameObject.SetActive(true);
                Component.Destroy(self);
                return;
            }
            else if (self.FsmName == "Conversation Control" && self.gameObject.name == "Dreamer Plaque Inspect")
            {
                self.GetState("Hero Anim").RemoveActions<ActivateGameObject>();
                self.GetState("Hero Anim").AdjustTransitions("Map Msg?");
            }
            else if (self.FsmName == "Control" && self.gameObject.name == "Dreamer Scene 2")
            {
                self.GetState("Take Control").AdjustTransitions("Fade Out");
                self.GetState("Fade Out").AdjustTransitions("Set Compass Point");
            }
        }

        orig(self);
    }

    private void PlayerDataBoolTest_OnEnter(On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.orig_OnEnter orig, HutongGames.PlayMaker.Actions.PlayerDataBoolTest self)
    {
        if (IsModuleUsed && ((self.IsCorrectContext("Control", "IK Remains", "Check")
            && self.boolName.Value == "infectedKnightDreamDefeated") || (self.IsCorrectContext("Control", "Mage Lord Remains", "Check")
            && self.boolName.Value == "mageLordDreamDefeated") || (self.IsCorrectContext("Control", "FK Corpse", "Check")
            && self.boolName.Value == "falseKnightDreamDefeated")
            || self.IsCorrectContext("Conversation Control", "Dreamer Plaque Inspect", "End")
            || self.IsCorrectContext("Control", "Dreamer Scene 2", "Init")
            || self.IsCorrectContext("FSM", "PostDreamnail", "Check")))
            self.isTrue = self.isFalse;
        orig(self);
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene newScene)
    {
        if (IsModuleUsed)
        {
            if (newScene.name == "Abyss_18")
                GameObject.Find("door_dreamReturn").transform.position = new(44.1f, 28.4f);
            else if (newScene.name == "Ruins1_24")
                GameObject.Find("door_dreamReturn").transform.position = new(4.25f, 18.4f);
        }
        // To prevent a softlock we spawn the portal anyway, even if the module is not used, if the player is in the room with dream nail already.
        if (newScene.name == "Dream_Nailcollection" && PlayerData.instance.GetBool(nameof(PlayerData.hasDreamNail)))
        {
            GameObject teleporterSprite = GameObject.Instantiate(BossModule.DreamGate);
            teleporterSprite.transform.position = new(272.88f, 51.3f);
            GameObject teleporter = GameObject.Instantiate(ElevatorModule.Door);
            teleporter.name = "Dream Nail Escape";
            teleporter.transform.position = new(272.88f, 52.4f);
            teleporter.SetActive(true);
            teleporter.transform.localScale = new(0.5f, 1f, 1f);

            PlayMakerFSM teleportFsm = teleporter.GetComponent<PlayMakerFSM>();
            teleportFsm.FsmVariables.FindFsmString("Entry Gate").Value = "right1";
            teleportFsm.FsmVariables.FindFsmString("New Scene").Value = "RestingGrounds_07";
            teleportFsm.GetState("Enter").GetLastAction<SendEventByName>().sendEvent.Value = "FADE OUT";
            teleportFsm.FsmVariables.FindFsmString("Prompt Name").Value = "Exit";
            teleportFsm.FsmVariables.FindFsmBool("Crossroads Ascent").Value = false;
        }
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        On.DeactivateIfPlayerdataTrue.OnEnable += DeactivateIfPlayerdataTrue_OnEnable;
        On.DeactivateIfPlayerdataFalse.OnEnable += DeactivateIfPlayerdataFalse_OnEnable;
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter += PlayerDataBoolTest_OnEnter;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    internal override void Disable()
    {
        On.DeactivateIfPlayerdataTrue.OnEnable -= DeactivateIfPlayerdataTrue_OnEnable;
        On.DeactivateIfPlayerdataFalse.OnEnable -= DeactivateIfPlayerdataFalse_OnEnable;
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter -= PlayerDataBoolTest_OnEnter;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    #endregion
}
