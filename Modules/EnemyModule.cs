using HutongGames.PlayMaker.Actions;
using KorzUtils.Helper;
using Modding;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheHuntIsOn.Modules;

internal class EnemyModule : Module
{
    #region Properties

    public override string MenuDescription => "Disables all non-boss enemies. Makes bosses invincible.";

    #endregion

    #region Members

    private static GameObject _dreamGate;

    public static GameObject TeleporterPrefab { get; set; }

    public static GameObject DreamGate
    {
        get
        {
            if (_dreamGate == null)
                _dreamGate = HeroController.instance.gameObject.LocateMyFSM("Dream Nail")
                    .GetState("Spawn Gate")
                    .GetFirstAction<SpawnObjectFromGlobalPool>().gameObject.Value;

            return _dreamGate;
        }
    }

    public static GameObject FKDreamEnter { get; set; }
    public static GameObject STDreamEnter { get; set; }
    public static GameObject HKDreamEnter { get; set; }
    public static GameObject DreamTree { get; set; }

    #endregion

    #region Eventhandler

    private bool ModHooks_OnEnableEnemyHook(GameObject enemy, bool isAlreadyDead)
    {
        if (!IsModuleUsed)
            return isAlreadyDead;

        HealthManager healthManager = enemy.GetComponent<HealthManager>();

        if (healthManager.hp > 200 ||
            enemy.name == "Mega Moss Charger" ||
            enemy.name == "Giant Fly" ||
            enemy.name == "False Knight New" ||
            enemy.name == "Mage Knight" ||
            enemy.name == "Mage Lord Phase2" ||
            enemy.name == "Head" ||
            enemy.name == "Mantis Lord S1" ||
            enemy.name == "Mantis Lord S2" ||
            enemy.name == "Ghost Warrior Xero")
        {
            healthManager.hp = 9999;
            return false;
        }
        else if ((enemy.name.Contains("Fly") && enemy.scene.name == "Crossroads_04") ||
                  enemy.scene.name == "Fungus3_23_boss" ||
                  enemy.scene.name == "Ruins2_11_boss")
            return false;
        else if (enemy.name.StartsWith("Acid Walker") ||
                 enemy.scene.name.StartsWith("Room_Colosseum") ||
                 enemy.name == "Radiance")
            return false;

        return true;
    }

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
            if (self.gameObject.name == "Fk Break Wall" && self.FsmName == "Control")
                self.GetState("Pause").AdjustTransitions("Initial");
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
            }
        }

        orig(self);
    }

    private void PlayerDataBoolTest_OnEnter(On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.orig_OnEnter orig, HutongGames.PlayMaker.Actions.PlayerDataBoolTest self)
    {
        if (IsModuleUsed &&
            ((self.IsCorrectContext("Control", "IK Remains", "Check") && self.boolName.Value == "infectedKnightDreamDefeated") ||
            (self.IsCorrectContext("Control", "Mage Lord Remains", "Check") && self.boolName.Value == "mageLordDreamDefeated") ||
            (self.IsCorrectContext("Control", "FK Corpse", "Check") && self.boolName.Value == "falseKnightDreamDefeated") ||
            self.IsCorrectContext("Conversation Control", "Dreamer Plaque Inspect", "End") ||
            self.IsCorrectContext("Control", "Dreamer Scene 2", "Init") ||
            self.IsCorrectContext("FSM", "PostDreamnail", "Check")))
            self.isTrue = self.isFalse;

        orig(self);
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene newScene)
    {
        if (IsModuleUsed)
        {
            switch (newScene.name)
            {
                case "Crossroads_10":
                    GameObject FKcorpse = GameObject.Instantiate(DreamTree);
                    FKcorpse.transform.position = new Vector3(32.88f, 52.24f, 0.007f);
                    FKcorpse.SetActive(true);
                    GameObject FKentry = GameObject.Instantiate(FKDreamEnter);
                    FKentry.transform.position = new Vector3(32.88f, 48.84f, 0.007f);
                    FKentry.SetActive(true);
                    break;
                case "Ruins1_24":
                    GameObject.Find("door_dreamReturn").transform.position = new Vector3(4.1f, 18.65f, 0.007f);
                    GameObject STcorpse = GameObject.Instantiate(DreamTree);
                    STcorpse.transform.position = new Vector3(4.1f, 22.35f, 0.007f);
                    STcorpse.SetActive(true);
                    GameObject STentry = GameObject.Instantiate(STDreamEnter);
                    STentry.transform.position = new Vector3(4.1f, 18.65f, 0.007f);
                    STentry.SetActive(true);
                    break;
                case "Dream_02_Mage_Lord":
                    CreateTeleporter(new Vector3(39.20f, 10.4f), "Soul Tyrant Exit", "Ruins1_24", "left1");
                    break;
                case "Dream_Mighty_Zote":
                    CreateTeleporter(new Vector3(9.1f, 6.4f), "Grey Prince Zote Exit", "Room_Bretta_Basement", "top1");
                    break;
                case "Dream_Nailcollection":
                    CreateTeleporter(new Vector3(272.88f, 52.4f), "Dream Nail Escape", "RestingGrounds_07", "right1");
                    break;
                case "Room_Final_Boss_Core":
                    GameObject HKcorpse = GameObject.Instantiate(DreamTree);
                    HKcorpse.transform.position = new Vector3(15.0f, 10.4f, 0.007f);
                    HKcorpse.SetActive(true);
                    GameObject HKentry = GameObject.Instantiate(HKDreamEnter);
                    HKentry.transform.position = new Vector3(15.0f, 7.31f, 0.007f);
                    HKentry.SetActive(true);
                    GameManager.instance.StartCoroutine(ModifyRadianceRoom());
                    break;
                case "Dream_Final_Boss":
                    CreateTeleporter(new Vector3(50.0f, 21.4f), "Radiance Exit (Start)", "Room_Final_Boss_Core", "left1");
                    CreateTeleporter(new Vector3(42.3f, 36.7f), "Radiance Exit (Plats)", "Room_Final_Boss_Core", "left1");
                    CreateTeleporter(new Vector3(63.3f, 138.2f), "Radiance Exit (Climb)", "Room_Final_Boss_Core", "left1");
                    GameManager.instance.StartCoroutine(ModifyRadianceRoom());
                    break;
                default:
                    break;
            }
        }
    }

    private void BeginSceneTransition_OnEnter(On.HutongGames.PlayMaker.Actions.BeginSceneTransition.orig_OnEnter orig, HutongGames.PlayMaker.Actions.BeginSceneTransition self)
    {
        if (IsModuleUsed && self.IsCorrectContext("Door Control", "Radiance Enter", "Change Scene"))
            GameManager.instance.StartCoroutine(ModifyRadianceRoom());
        orig(self);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a teleporter from a player location going to an entry location in a scene.
    /// </summary>
    /// <param name="playerLocation">The standing x,y coordinates of the player where the portal should be placed.</param>
    /// <param name="portalName">The name of the portal.</param>
    /// <param name="sceneTo">The name of the scene the portal should send the player to.</param>
    /// <param name="entryLocation">The transition the player should appear from within the specified scene.</param>
    private void CreateTeleporter(Vector3 playerLocation, string portalName, string sceneTo, string entryLocation)
    {
        var portalInteractionLocation = new Vector3(playerLocation.x, (playerLocation.y - 0.31f));
        var portalSpriteLocation = new Vector3(playerLocation.x, (playerLocation.y - 1.51f));

        GameObject teleporterSprite = GameObject.Instantiate(DreamGate);
        teleporterSprite.transform.position = portalSpriteLocation;
        GameObject teleporter = GameObject.Instantiate(ElevatorModule.Door);
        teleporter.name = portalName;
        teleporter.transform.position = portalInteractionLocation;
        teleporter.SetActive(true);
        teleporter.transform.localScale = new(0.5f, 1f, 1f);

        PlayMakerFSM teleportFsm = teleporter.GetComponent<PlayMakerFSM>();
        teleportFsm.FsmVariables.FindFsmString("Entry Gate").Value = entryLocation;
        teleportFsm.FsmVariables.FindFsmString("New Scene").Value = sceneTo;
        teleportFsm.FsmVariables.FindFsmString("Prompt Name").Value = "Exit";
        teleportFsm.FsmVariables.FindFsmBool("Crossroads Ascent").Value = false;
        teleportFsm.GetState("Enter").GetLastAction<SendEventByName>().sendEvent.Value = "FADE OUT";
        teleportFsm.GetState("Change Scene").GetFirstAction<BeginSceneTransition>().preventCameraFadeOut = true;
    }

    private IEnumerator ModifyRadianceRoom()
    {
        string finalBossRoom = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        while (currentScene == finalBossRoom)
        {
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            yield return null;
        }
        if (currentScene != "Dream_Final_Boss")
            yield break;
        // Since entering dreams doesn't lift the fade plane, we remove them manually.
        yield return new WaitForFinishedEnteringScene();
        GameManager.instance.FadeSceneIn();

        // Let the UI reappear.
        GameObject.Find("_GameCameras/HudCamera/Hud Canvas").LocateMyFSM("Slide Out").SendEvent("IN");
    }

    internal override void Enable()
    {
        ModHooks.OnEnableEnemyHook += ModHooks_OnEnableEnemyHook;
        On.DeactivateIfPlayerdataTrue.OnEnable += DeactivateIfPlayerdataTrue_OnEnable;
        On.DeactivateIfPlayerdataFalse.OnEnable += DeactivateIfPlayerdataFalse_OnEnable;
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter += PlayerDataBoolTest_OnEnter;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        On.HutongGames.PlayMaker.Actions.BeginSceneTransition.OnEnter += BeginSceneTransition_OnEnter;
    }

    internal override void Disable()
    {
        ModHooks.OnEnableEnemyHook -= ModHooks_OnEnableEnemyHook;
        On.DeactivateIfPlayerdataTrue.OnEnable -= DeactivateIfPlayerdataTrue_OnEnable;
        On.DeactivateIfPlayerdataFalse.OnEnable -= DeactivateIfPlayerdataFalse_OnEnable;
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter -= PlayerDataBoolTest_OnEnter;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        On.HutongGames.PlayMaker.Actions.BeginSceneTransition.OnEnter -= BeginSceneTransition_OnEnter;
    }

    #endregion
}
