using KorzUtils.Helper;
using Mono.Security.Protocol.Tls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheHuntIsOn.Modules;

internal class ElevatorModule : Module
{
    #region Properties

    public override string MenuDescription => "Removes elevators in City of Tears.";

    public static GameObject Door { get; set; }

    public static bool HasClaw => PlayerData.instance.GetBool(nameof(PlayerData.hasWalljump));

    public static bool HasWings => PlayerData.instance.GetBool(nameof(PlayerData.hasDoubleJump));

    private GameObject _teleporter;

    #endregion

    #region Eventhandler

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed && (self.gameObject.name.StartsWith("Lift Call Lever") || self.gameObject.name.StartsWith("Ruins Lift")))
        {
            GameObject.Destroy(self.gameObject);
            return;
        }
        orig(self);
    }

    private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene newScene)
    {
        if (!IsModuleUsed)
            return;
        if (newScene.name == "Crossroads_49b")
        {
            GameObject entryPoint = new("left2");
            entryPoint.transform.position = new(18.43f, 5.01f);
            TransitionPoint transitionPoint = entryPoint.AddComponent<TransitionPoint>();
            transitionPoint.isADoor = false;
            transitionPoint.entryPoint = "left2";
            transitionPoint.targetScene = "Crossroads_49";
            transitionPoint.respawnMarker = entryPoint.AddComponent<HazardRespawnMarker>();

            _teleporter = GameObject.Instantiate(Door);
            _teleporter.transform.position = new(15.11f, 5.41f);
            _teleporter.SetActive(PlayerData.instance.GetBool(nameof(PlayerData.cityLift1)));
            PlayMakerFSM fsm = _teleporter.LocateMyFSM("Door Control");
            fsm.FsmVariables.FindFsmString("New Scene").Value = "Crossroads_49";
            fsm.FsmVariables.FindFsmBool("Crossroads Ascent").Value = false;
            fsm.FsmVariables.FindFsmString("Entry Gate").Value = "left1";
            fsm.GetState("Change Scene").AddActions(() => GameManager.instance.StartCoroutine(AdjustStartPosition(new(14.8f, 158.4f), "Crossroads_49")));
        }
        else if (newScene.name == "Crossroads_49")
        {
            GameObject transporter = GameObject.Instantiate(Door);
            transporter.transform.position = new(14.8f, 158.98f);
            transporter.SetActive(true);
            PlayMakerFSM fsm = transporter.LocateMyFSM("Door Control");
            fsm.FsmVariables.FindFsmString("New Scene").Value = "Crossroads_49b";
            fsm.FsmVariables.FindFsmString("Prompt Name").Value = "Exit";
            fsm.FsmVariables.FindFsmBool("Crossroads Ascent").Value = false;
            fsm.FsmVariables.FindFsmString("Entry Gate").Value = "left2";
        }
        else if (newScene.name == "Ruins2_10b")
        {
            GameObject entryPoint = new("left2");
            entryPoint.transform.position = new(17.43f, 10.41f);
            TransitionPoint transitionPoint = entryPoint.AddComponent<TransitionPoint>();
            transitionPoint.isADoor = false;
            transitionPoint.entryPoint = "left2";
            transitionPoint.targetScene = "Ruins2_10";
            transitionPoint.respawnMarker = entryPoint.AddComponent<HazardRespawnMarker>();

            GameObject transporter = GameObject.Instantiate(Door);
            transporter.transform.position = new(14.94f, 10.41f);
            transporter.SetActive(true);
            PlayMakerFSM fsm = transporter.LocateMyFSM("Door Control");
            fsm.FsmVariables.FindFsmString("New Scene").Value = "Ruins2_10";
            fsm.FsmVariables.FindFsmBool("Crossroads Ascent").Value = false;
            fsm.FsmVariables.FindFsmString("Entry Gate").Value = "right1";
            fsm.GetState("Change Scene").AddActions(() => GameManager.instance.StartCoroutine(AdjustStartPosition(new(11.93f, 158.4f), "Ruins2_10")));
        }
        else if (newScene.name == "Ruins2_10")
        {
            GameObject transporter = GameObject.Instantiate(Door);
            transporter.transform.position = new(14.92f, 158.98f);
            transporter.SetActive(true);
            PlayMakerFSM fsm = transporter.LocateMyFSM("Door Control");
            fsm.FsmVariables.FindFsmString("New Scene").Value = "Ruins2_10b";
            fsm.FsmVariables.FindFsmString("Prompt Name").Value = "Exit";
            fsm.FsmVariables.FindFsmBool("Crossroads Ascent").Value = false;
            fsm.FsmVariables.FindFsmString("Entry Gate").Value = "left2";
        }
        else if (!HasClaw || !HasWings)
        {
            // As c# doesn't allow variable to be declared in different switch cases with the same name and I'm lazy, we put the other scenes
            // in a extra switch table.
            switch (newScene.name)
            {
                case "Ruins1_02" when !HasClaw:
                    CreatePlatformLadder(new(3f, 16f), new(7.73f, 37.25f));
                    break;
                case "Ruins1_03":
                    CreatePlatformLadder(new(17.26f, 12.8f), new(21.73f, 50.09f));
                    break;
                case "Ruins1_05b":
                    CreatePlatformLadder(new(13.26f, 10.4f), new(17.73f, 22.35f));
                    break;
                case "Ruins1_05c":
                    CreatePlatformLadder(new(38.26f, 40.4f), new(42.73f, 98.8f), false);
                    break;
                case "Ruins1_05" when !HasClaw:
                    CreatePlatformLadder(new(38.26f, 123.4f), new(42.73f, 150.4f));
                    break;
                case "Ruins1_31" when !HasClaw:
                    CreatePlatformLadder(new(47.26f, 7.4f), new(51.7f, 24.71f));
                    break;
                case "Ruins1_23" when !HasClaw:
                    CreatePlatformLadder(new(29.26f, 14.57f), new(33.73f, 26.4f));
                    CreatePlatformLadder(new(4.3f, 31.6f), new(8.7f, 72.71f));
                    break;
                case "Ruins1_25" when !HasClaw:
                    CreatePlatformLadder(new(25.26f, 67.31f), new(29.66f, 83.8f));
                    break;
                case "Ruins2_03b" when !HasClaw:
                    CreatePlatformLadder(new(38.26f, 17.4f), new(42.72f, 36.8f));
                    break;
                case "Ruins2_03" when !HasClaw:
                    CreatePlatformLadder(new(5.94f, 42.4f), new(10.73f, 68.6f));
                    break;
                case "Ruins2_Watcher_Room" when !HasClaw:
                    CreatePlatformLadder(new(28.26f, 6.8f), new(31.7f, 133.17f));
                    break;
                case "Ruins2_01_b":
                    CreatePlatformLadder(new(9.26f, 9.42f), new(13.73f, 26.76f));
                    CreatePlatformLadder(new(64.26f, 9.42f), new(68.73f, 26.76f));
                    break;
                case "Ruins_Elevator":
                    CreatePlatformLadder(new(28.26f, 10f), new(32.73f, 138.24f));
                    break;
                case "Ruins2_05" when !HasClaw:
                    CreatePlatformLadder(new(7.26f, 17f), new(11.73f, 42.47f));
                    CreatePlatformLadder(new(36.26f, 37.5f), new(40.73f, 72.46f), false);
                    break;
                default:
                    break;
            }
        }
    }

    private void SetPlayerDataBool_OnEnter(On.HutongGames.PlayMaker.Actions.SetPlayerDataBool.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SetPlayerDataBool self)
    {
        if (IsModuleUsed && self.IsCorrectContext("Toll Machine", "Toll Machine Lift", "Send Message"))
            _teleporter.SetActive(true);
        orig(self);
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        On.HutongGames.PlayMaker.Actions.SetPlayerDataBool.OnEnter += SetPlayerDataBool_OnEnter;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        
    }

    internal override void Disable()
    {
        On.HutongGames.PlayMaker.Actions.SetPlayerDataBool.OnEnter -= SetPlayerDataBool_OnEnter;
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    private IEnumerator AdjustStartPosition(Vector3 startPosition, string sceneToWait)
    {
        yield return new WaitUntil(() => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneToWait);
        yield return new WaitForFinishedEnteringScene();
        HeroController.instance.transform.position = startPosition;
    }

    private void CreatePlatformLadder(Vector2 bottomLeft, Vector2 topRight, bool placeLeft = true)
    {
        float currentHeight = bottomLeft.y - 1f;
        while (currentHeight <= topRight.y)
        {
            GameObject platform = GameObject.Instantiate(ShadeSkipModule.PlatformPrefab, new(placeLeft ? bottomLeft.x : topRight.x, currentHeight, 0f), Quaternion.identity);
            platform.SetActive(true);
            currentHeight += 5f;
            placeLeft = !placeLeft;
        }
    }

    #endregion
}
