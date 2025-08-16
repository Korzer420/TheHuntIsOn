using HutongGames.PlayMaker.Actions;
using KorzUtils.Helper;
using Modding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheHuntIsOn.Modules;

internal class CompletionModule : Module
{
    #region Members

    private List<string> _affectedpd = new()
    {
       "gladeDoorOpened",
       /*"openedTown",
       "openedCrossroads",
       "openedGreenpath",
       "openedRuins1",
       "openedRuins2",
       "openedFungalWastes",
       "openedRoyalGardens",
       "openedRestingGrounds",
       "openedDeepnest",
       "openedStagNest",
       "openedHiddenStation",*/
       "oneWayArchive",
       "cityBridge1",
       "cityBridge2",
       "cityLift1",
       "cityLift2",
       "openedMageDoor",
       "openedMageDoor_v2",
       "brokenMageWindow",
       "brokenMageWindowGlass",
       "ruins1_5_tripleDoor",
       "openedCityGate",
       "city2_sewerDoor",
       "openedLoveDoor",
       "completedQuakeArea",
       "waterwaysGate",
       "openedWaterwaysManhole",
       "waterwaysAcidDrained",
       "dungDefenderWallBroken",
       "brokeMinersWall",
       "steppedBeyondBridge",
       "deepnestBridgeCollapsed",
       "spiderCapture",
       "deepnest26b_switch",
       "openedRestingGrounds02",
       "restingGroundsCryptWall",
       "openedGardensStagStation",
       "blizzardEnded",
       "abyssGateOpened",
       "abyssLighthouse",
       "blueVineDoor",
       "shamanPillar",
       "brettaRescued",
       "zoteDefeated",
       "bathHouseWall",
       "brokeMinersWall",
       "crossroadsMawlekWall",
       "deepnestWall",
       "dungDefenderWallBroken",
       "outskirtsWall",
       "falseKnightWallBroken",
       "zoteStatueWallBroken",
       "giantFlyDefeated",
       "blocker1Defeated",
       "blocker2Defeated",
       "encounteredHornet",
       "savedByHornet",
       "mageLordEncountered",
       "mageLordEncountered_2",
       "duskKnightDefeated",
       "flukeMotherEncountered",
       "defeatedDoubleBlockers",
       "defeatedNightmareGrimm",
       "visitedDirtmouth",
       "visitedCrossroads",
       "visitedGreenpath",
       "visitedFungus",
       "visitedHive",
       "visitedCrossroadsInfected",
       "visitedRuins",
       "visitedMines",
       "visitedRoyalGardens",
       "visitedWaterways",
       "visitedAbyss",
       "visitedOutskirts",
       "visitedWhitePalace",
       "visitedCliffs",
       "visitedAbyss",
       "visitedAbyssLower",
       "visitedGodhome",
       "visitedMines10",
       "openedBlackEggDoor",
       "whitePalaceOrb_1",
       "whitePalaceOrb_2",
       "whitePalaceOrb_3",
       "godseekerUnlocked",
       "colosseumBronzeOpened",
       "colosseumBronzeCompleted",
       "colosseumSilverOpened",
       "colosseumSilverCompleted",
       "colosseumGoldOpened",
       "colosseumGoldCompleted",
       "mineLiftOpened",
       "slyRescued",
       "openedMapperShop"
    };
    private List<string> _affectedBosspd = new()
    {
        "killedInfectedKnight", // Boss Flag
        "mawlekDefeated", // Boss Flag
        "collectorDefeated", // Boss Flag
        "defeatedMegaBeamMiner", // Boss Flag
        "defeatedDungDefender", // Boss Flag
        "falseKnightDefeated", // Boss Flag
        "flukeMotherDefeated", // Boss Flag
        "foughtGrimm", // Boss Flag
        "giantFlyDefeated", // Boss Flag
        "killedHiveKnight", // Boss Flag
        "hornet1Defeated", // Boss Flag
        "defeatedMantisLords", // Boss Flag
        "megaMossChargerDefeated", // Boss Flag
        "killedMimicSpider", // Boss Flag
        "mageLordDefeated", // Boss Flag
        "killedMageKnight", // Boss Flag
        "killedTraitorLord", // Boss Flag
        "defeatedMegaJelly", // Boss Flag
        "giantBuzzerDefeated", // Boss Flag
        "defeatedMegaBeamMiner2", // Boss Flag
        "hornetOutskirtsDefeated", // Boss Flag
        "zoteDefeated", // Boss Flag
        "guardiansDefeated", // Boss Flag
        "defeatedNightmareGrimm", // Boss Flag
        "xeroDefeated", // Boss Flag
        "aladarSlugDefeated", // Boss Flag
        "elderHuDefeated", // Boss Flag
        "duskKnightDefeated", // Boss Flag
        "mumCaterpillarDefeated", // Boss Flag
        "noEyesDefeated", // Boss Flag
        "galienDefeated", // Boss Flag
        "markothDefeated" // Boss Flag
    };
    private List<SceneBoolData> _affectedBoolData = new();

    #endregion

    #region Constructors

    public CompletionModule()
    {
        _affectedBoolData = ResourceHelper.LoadJsonResource<TheHuntIsOn, List<SceneBoolData>>("SceneData.json");
    }

    #endregion

    #region Properties

    public override string MenuDescription => "Unlocks Hallownest as a whole.";

    #endregion

    #region Eventhandler

    private bool ModHooks_GetPlayerBoolHook(string name, bool orig)
    {
        if (!IsModuleUsed)
            return orig;
        if (_affectedpd.Contains(name))
            return true;
        else if (_affectedBosspd.Contains(name))
        {
            return false;
        }
        else
            return orig;
    }

    private PersistentBoolData SceneData_FindMyState_PersistentBoolData(On.SceneData.orig_FindMyState_PersistentBoolData orig, SceneData self, PersistentBoolData persistentBoolData)
    {
        if (!IsModuleUsed || _affectedBoolData.FirstOrDefault(x => x.id == persistentBoolData.id && x.sceneName == persistentBoolData.sceneName) is not SceneBoolData sceneBool)
            return orig(self, persistentBoolData);
        if (sceneBool.BossFlag)
            return orig(self, persistentBoolData);
        else
            return sceneBool;
    }

    private void IntCompare_OnEnter(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntCompare self)
    {
        if (IsModuleUsed && self.IsCorrectContext("Destroy if Quake", "Battle Gate (1)", "Check"))
            self.lessThan = self.greaterThan;
        orig(self);
    }

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed)
        {
            // Prevents Vengeful Spirit pickup
            if (self.gameObject.name == "Knight Cutscene Animator" && self.FsmName == "Check Fall")
            {
                self.GetState("Set Respawns").RemoveActions<SetPlayerDataBool>();
                self.GetState("Set Respawns").RemoveActions<SetPlayerDataInt>();
                self.GetState("Set Respawns").AdjustTransitions("Fade Back");
            }

            // Prevents Howling Wraiths pickup
            if (self.gameObject.name == "Knight Get Scream" && self.FsmName == "Get Scream")
            {
                self.GetState("Start").RemoveActions<SetPlayerDataBool>();
                self.GetState("Start").RemoveActions<SetPlayerDataInt>();
                self.GetState("Fall").AdjustTransitions("Get Up");
            }

            // Prevents Desolate Dive pickup
            if (self.gameObject.name == "Quake Pickup" && self.FsmName == "Pickup")
            {
                self.GetState("Idle").AdjustTransitions("Dormant");
            }

            // Prevents Shade Soul pickup
            if (self.gameObject.name == "Knight Get Fireball Lv2" && self.FsmName == "Get Fireball")
            {
                self.GetState("Get PlayerData").RemoveActions<SetPlayerDataInt>();
                self.GetState("Get PlayerData").AdjustTransitions("Get Up Anim");
            }

            // Prevents Descending Dark pickup
            if (self.gameObject.name == "Crystal Shaman" && self.FsmName == "Control")
            {
                self.GetState("Hit").RemoveActions<IntAdd>();
            }

            // Prevents Abyss Shriek pickup
            if (self.gameObject.name == "Scream 2 Get" && self.FsmName == "Scream Get")
            {
                self.GetState("Init").AdjustTransitions("Inert");
            }

            // Prevents Monarch Wings pickup
            if (self.gameObject.name == "Shiny Item DJ" && self.FsmName == "FSM")
            {
                self.GetState("Pause").AdjustTransitions("Destroy");
            }

            // Prevents Isma's Tear pickup
            if (self.gameObject.name == "Shiny Item Acid" && self.FsmName == "FSM")
            {
                self.GetState("Pause").AdjustTransitions("Destroy");
            }

            // Prevents Shade Cloak pickup
            if (self.gameObject.name == "Dish Plat" && self.FsmName == "Get Shadow Dash")
            {
                self.GetState("Pause").AdjustTransitions("Got");
            }

            // Prevents Crystal Heart pickup
            if (self.gameObject.name == "Super Dash Get" && self.FsmName == "FSM")
            {
                self.GetState("Pause").AdjustTransitions("Destroy");
            }

            // Removes door left of Mantis Lords
            if (self.gameObject.name == "mantis_big_door" && self.FsmName == "FSM")
            {
                self.GetState("Pause").AdjustTransitions("Destroy");
            }

            // Removes door at Mantis Lords
            if (self.gameObject.name == "deepnest_mantis_gate" && self.FsmName == "Gate Control")
            {
                self.GetState("Pause").AdjustTransitions("Open");
                self.GetState("Open").AdjustTransitions("Opened");
            }

            // Makes Hornet 2 accessible
            if (self.gameObject.name == "blizzard_wall" && self.FsmName == "FSM")
                self.GetState("Pause").AdjustTransitions("Deactivate");

            // Destroys Soul Master & Soul Tyrant Ground
            if ((self.gameObject.name == "Dream Mage Lord" || self.gameObject.name == "Mage Lord")
                        && self.FsmName == "Mage Lord")
            {
                GameObject.Find("mage_window").SetActive(false);
            }
        }

        orig(self);
    }

    private void ModHooks_NewGameHook()
    {
        if (IsModuleUsed)
        {
            var pd = PlayerData.instance;

            pd.maxHealth = 4;
            pd.maxHealthBase = 4;

            pd.lurienDefeated = true;
            pd.monomonDefeated = true;
            pd.hegemolDefeated = true;

            pd.charmSlots = 6;
            pd.gotCharm_36 = true;
            pd.gotCharm_3 = true;
            pd.royalCharmState = 3;
            pd.equippedCharm_3 = true;
            pd.equippedCharm_36 = true;

            pd.hasDreamNail = true;
            pd.dreamNailUpgraded = true;

            pd.hasLantern = true;
            pd.hasTramPass = true;
            pd.hasKingsBrand = true;

            pd.hasMap = true;
            pd.gladeDoorOpened = true;
            pd.troupeInTown = true;
            pd.geo = 9999;
        }
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        ModHooks.GetPlayerBoolHook += ModHooks_GetPlayerBoolHook;
        On.SceneData.FindMyState_PersistentBoolData += SceneData_FindMyState_PersistentBoolData;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += IntCompare_OnEnter;
        ModHooks.NewGameHook += ModHooks_NewGameHook;
    }

    internal override void Disable()
    {
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
        ModHooks.GetPlayerBoolHook -= ModHooks_GetPlayerBoolHook;
        On.SceneData.FindMyState_PersistentBoolData -= SceneData_FindMyState_PersistentBoolData;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter -= IntCompare_OnEnter;
        ModHooks.NewGameHook -= ModHooks_NewGameHook;
    }

    #endregion
}
