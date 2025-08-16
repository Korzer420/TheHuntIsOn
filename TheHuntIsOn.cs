using KorzUtils.Helper;
using Modding;
using Satchel.BetterMenus;
using Satchel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hkmp.Api.Client;
using Hkmp.Api.Server;
using TheHuntIsOn.HkmpAddon;
using TheHuntIsOn.Modules;
using TheHuntIsOn.Modules.HealthModules;
using UnityEngine;
using IL.InControl.NativeDeviceProfiles;
using Satchel.BetterPreloads;
using Modding.Menu.Config;

namespace TheHuntIsOn;

public class TheHuntIsOn : Mod, IGlobalSettings<HuntGlobalSaveData>, ICustomMenuMod
{
    #region Constructors

    public TheHuntIsOn() 
    { 
        Instance = this;
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter += PlayerDataBoolTest_OnEnter;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += IntCompare_OnEnter;
    }

    #endregion

    #region Properties

    public static TheHuntIsOn Instance { get; set; }

    public static HuntGlobalSaveData SaveData { get; set; } = new();

    public bool ToggleButtonInsideMenu { get; }

    public Menu MenuRef { get; set; }
    
    internal List<Module> Modules { get; set; } = new()
    {
        new AutoTriggerBossModule(),
        new BaldurModule(),
        new BenchModule(),
        new EnemyModule(),
        new CharmNerfModule(),
        new CompletionModule(),
        new CutsceneSkipModule(),
        new DisableSoulGainModule(),
        new DreamHealModule(),
        new ElevatorModule(),
        new EventNetworkModule(),
        new HelperPlatformModule(),
        new IntangibleGatesModule(),
        new InvisibleGatesModule(),
        new LifeseedModule(),
        new MaskModule(),
        new NotchModule(),
        new RespawnModule(),
        new ShadeModule(),
        new ShadeSkipModule(),
        new SpaModule(),
        new StagModule(),
        new TramModule()
    };

    #endregion

    #region Mod Setup

    public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    public override List<(string, string)> GetPreloadNames() => new()
    {
        ("Crossroads_04", "_Scenery/plat_float_01"),
        ("White_Palace_03_hub", "doorWarp"),
        ("Crossroads_01", "_Transition Gates/door1"),
        ("Crossroads_10_boss_defeated", "Prayer Room/FK Corpse/Dream Enter"),
        ("Ruins1_24_boss_defeated", "Mage Lord Remains/Dream Enter"),
        ("Room_Final_Boss_Core", "Boss Control/Hollow Knight Boss/Dream Enter"),
        ("Crossroads_07", "Dream Plant")
    };

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        SetupHKMP();
        On.UIManager.StartNewGame += UIManager_StartNewGame;
        On.UIManager.ContinueGame += UIManager_ContinueGame;
        On.UIManager.ReturnToMainMenu += UIManager_ReturnToMainMenu;
        ShadeSkipModule.PlatformPrefab = preloadedObjects["Crossroads_04"]["_Scenery/plat_float_01"];
        EnemyModule.TeleporterPrefab = preloadedObjects["White_Palace_03_hub"]["doorWarp"];
        ElevatorModule.Door = preloadedObjects["Crossroads_01"]["_Transition Gates/door1"];
        EnemyModule.FKDreamEnter = preloadedObjects["Crossroads_10_boss_defeated"]["Prayer Room/FK Corpse/Dream Enter"];
        EnemyModule.STDreamEnter = preloadedObjects["Ruins1_24_boss_defeated"]["Mage Lord Remains/Dream Enter"];
        EnemyModule.HKDreamEnter = preloadedObjects["Room_Final_Boss_Core"]["Boss Control/Hollow Knight Boss/Dream Enter"];
        EnemyModule.DreamTree = preloadedObjects["Crossroads_07"]["Dream Plant"];
    }

    private void SetupHKMP()
    {
        // Some funky code to get the EventNetworkModule from the list of modules
        // Alternatively, the EventNetworkModule could be stored in the class
        ((EventNetworkModule)Modules.Find(module => module.GetType() == typeof(EventNetworkModule))).Initialize();
    }
    
    #endregion

    #region Eventhandler

    private System.Collections.IEnumerator UIManager_ReturnToMainMenu(On.UIManager.orig_ReturnToMainMenu orig, UIManager self)
    {
        yield return orig(self);
        foreach (Module module in Modules)
            module.Unload();
    }

    private void UIManager_ContinueGame(On.UIManager.orig_ContinueGame orig, UIManager self)
    {
        orig(self);
        KorzUtils.Helper.CoroutineHelper.WaitForHero(() => HealthControl.Reset(PlayerData.instance.GetInt(nameof(PlayerData.maxHealth))) , true);
        foreach (Module module in Modules)
            module.Load();
    }

    private void UIManager_StartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
    {
        orig(self, permaDeath, bossRush);
        HealthControl.Reset(5);
        foreach (Module module in Modules)
            module.Load();
    }

    private void PlayerDataBoolTest_OnEnter(On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.orig_OnEnter orig, HutongGames.PlayMaker.Actions.PlayerDataBoolTest self)
    {
        if (self.IsCorrectContext("Spell Control", "Knight", "Slug?"))
            if (SaveData.IsHunter)
                self.Fsm.Variables.FindFsmFloat("Time Per MP Drain").Value *= SaveData.FocusSpeed;
            else
                self.Fsm.Variables.FindFsmFloat("Time Per MP Drain").Value *= 1;
        else if (SaveData.IsHunter &&
                ((self.IsCorrectContext("Spell Control", "Knight", "Spore Cloud") || self.IsCorrectContext("Spell Control", "Knight", "Spore Cloud 2"))))
            HeroController.instance.TakeMP(SaveData.FocusCost - 33);
        else if (SaveData.IsHunter &&
                (self.IsCorrectContext("Spell Control", "Knight", "Fireball 1") ||
                 self.IsCorrectContext("Spell Control", "Knight", "Fireball 2") ||
                 self.IsCorrectContext("Spell Control", "Knight", "Level Check 2") ||
                 self.IsCorrectContext("Spell Control", "Knight", "Scream Burst 1") ||
                 self.IsCorrectContext("Spell Control", "Knight", "Scream Burst 2")))
        {
            if (!PlayerData.instance.equippedCharm_33)
                HeroController.instance.TakeMP(SaveData.SpellCost - 33);
            else
                HeroController.instance.TakeMP(SaveData.SpellCost - 24);
        }

        orig(self);
    }

    private void IntCompare_OnEnter(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntCompare self)
    {
        if (self.IsCorrectContext("Spell Control", "Knight", null) && self.integer1.Name == "MP" && (self.State.Name == "Can Focus?"
            || self.State.Name == "Full HP?" || self.State.Name == "Full HP? 2"))
            if (SaveData.IsHunter)
                self.integer2.Value = SaveData.FocusCost;
            else
                self.integer2.Value = 33;
        else if (self.IsCorrectContext("Spell Control", "Knight", "Can Cast? QC") || self.IsCorrectContext("Spell Control", "Knight", "Can Cast?"))
        {
            if (SaveData.IsHunter)
                self.Fsm.Variables.FindFsmInt("MP Cost").Value = SaveData.SpellCost;
            else
                self.Fsm.Variables.FindFsmInt("MP Cost").Value = 33;
        }

        orig(self);
    }

    #endregion

    #region Methods

    internal static bool IsModuleUsed<T>() where T : Module 
        => Instance.Modules.FirstOrDefault(x => x is T)?.IsModuleUsed ?? false;

    #endregion

    #region Interfaces

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
    {
        List<Element> elements = new()
        {
            new HorizontalOption("Role:", "Flag that indicates if player is a hunter or speedrunner.", new string[]{"Hunter", "Speedrunner" },
            x => SaveData.IsHunter = x == 0,
            () => SaveData.IsHunter ? 0 : 1),
            new CustomSlider("Hunter Focus Cost:", x => SaveData.FocusCost = (int)x, () => SaveData.FocusCost, 33, 99, true),
            new CustomSlider("Hunter Focus Speed:", x => SaveData.FocusSpeed = x, () => SaveData.FocusSpeed, 1, 3),
            new CustomSlider("Hunter Spell Cost:", x => SaveData.SpellCost = (int)x, () => SaveData.SpellCost, 33, 99, true),
        };
        foreach (Module module in Modules)
        {
            elements.Add(new HorizontalOption(module.GetModuleName(), module.MenuDescription, new string[] { "Off", "Both", "Hunter", "Speedrunner" },
            x => module.Affection = (ModuleAffection)x,
            () => (int)module.Affection));
        }
        elements.Add(new TextPanel("EnemyModule Toggles:", 1000, 35));
        elements.Add(new HorizontalOption("Disable Enemies", "Disables basic enemy spawns (with exceptions).", new string[] { "Off", "On" },
            x => Module.DisableEnemies = x == 1,
            () => Module.DisableEnemies ? 1 : 0));
        elements.Add(new HorizontalOption("Invincible Bosses", "Sets boss HP to 9999.", new string[] { "Off", "On" },
            x => Module.InvincibleBosses = x == 1,
            () => Module.InvincibleBosses ? 1 : 0));
        elements.Add(new HorizontalOption("Dream Boss Access", "Creates new entrances to dream boss scenes.", new string[] { "Off", "On" },
            x => Module.DreamBossAccess = x == 1,
            () => Module.DreamBossAccess ? 1 : 0));
        MenuRef ??= new("The Hunt Is On", elements.ToArray());
        return MenuRef.GetMenuScreen(modListMenu);
    }

    public void OnLoadGlobal(HuntGlobalSaveData saveData)
    {
        SaveData = saveData ?? new();
        SaveData.AffectionTable ??= [];
        
        foreach (Module module in Modules)
            if (SaveData.AffectionTable.ContainsKey(module.GetType().Name))
                module.Affection = SaveData.AffectionTable[module.GetType().Name];
        Module.DisableEnemies = SaveData.DisableEnemies;
        Module.InvincibleBosses = SaveData.InvincibleBosses;
        Module.DreamBossAccess = SaveData.DreamBossAccess;
    }

    public HuntGlobalSaveData OnSaveGlobal()
    {
        HuntGlobalSaveData globalData = new HuntGlobalSaveData();

        globalData.FocusCost = SaveData.FocusCost;
        globalData.FocusSpeed = SaveData.FocusSpeed;
        globalData.SpellCost = SaveData.SpellCost;
        globalData.IsHunter = SaveData.IsHunter;
        globalData.DisableEnemies = Module.DisableEnemies;
        globalData.InvincibleBosses = Module.InvincibleBosses;
        globalData.DreamBossAccess = Module.DreamBossAccess;

        foreach (Module module in Modules)
            globalData.AffectionTable.Add(module.GetType().Name, module.Affection);
        return globalData;
    }

    #endregion
}