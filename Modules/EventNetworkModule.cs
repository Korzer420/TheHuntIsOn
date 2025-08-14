using System;
using System.Linq;
using Hkmp.Api.Client;
using Hkmp.Api.Server;
using KorzUtils.Helper;
using Modding;
using Satchel;
using TheHuntIsOn.HkmpAddon;

namespace TheHuntIsOn.Modules;

internal class EventNetworkModule : Module
{
    #region Properties

    public override string MenuDescription => "Networks speedrunner caused events and grants items to hunters.";

    public HuntClientAddon HuntClientAddon { get; private set; }

    public HuntServerAddon HuntServerAddon { get; private set; }


    public static IClientApi _clientApi;
    
    private bool AreAddonsLoaded { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initialize method used to register HKMP client and server addons after the module affections has been set.
    /// If the module should not affect any players, we do not register the addons. Otherwise, we do.
    /// This way if module affection is set to none and the game is restarted, players can connect to server that
    /// do not have the TheHuntIsOn server addon.
    /// </summary>
    internal override void Initialize()
    {
        if (Affection == ModuleAffection.None)
        {
            AreAddonsLoaded = false;
            return;
        }

        HuntClientAddon = new HuntClientAddon();
        HuntServerAddon = new HuntServerAddon();
        ClientAddon.RegisterAddon(HuntClientAddon);
        ServerAddon.RegisterAddon(HuntServerAddon);

        AreAddonsLoaded = true;
    }

    #endregion

    #region EventHandler

    private int ModHooks_OnSetPlayerIntHook(string name, int orig)
    {
        // Make sure that the player causing changes is the speedrunner
        if (!IsModuleUsed || TheHuntIsOn.GlobalSaveData.IsHunter)
        {
            return orig;
        }

        if (name == nameof(PlayerData.fireballLevel))
        {
            if (orig == 1)
            {
                // Speedrunner obtained Vengeful Spirit
                SendEvent(NetEvent.VengefulSpirit);
                SendEvent(NetEvent.PowerUp);
            }
            else if (orig == 2)
            {
                // Speedrunner obtained Shade Soul
                SendEvent(NetEvent.ShadeSoul);
                SendEvent(NetEvent.PowerUp);
            }
        }
        else if (name == nameof(PlayerData.quakeLevel))
        {
            if (orig == 1)
            {
                // Speedrunner obtained Desolate Dive
                SendEvent(NetEvent.DesolateDive);
                SendEvent(NetEvent.PowerUp);
            }
            else if (orig == 2)
            {
                // Speedrunner obtained Descending Dark
                SendEvent(NetEvent.DescendingDark);
                SendEvent(NetEvent.PowerUp);
            }
        }
        else if (name == nameof(PlayerData.screamLevel))
        {
            if (orig == 1)
            {
                // Speedrunner obtained Howling Wraiths
                SendEvent(NetEvent.HowlingWraiths);
                SendEvent(NetEvent.PowerUp);
            }
            else if (orig == 2)
            {
                // Speedrunner obtained Abyss Shriek
                SendEvent(NetEvent.AbyssShriek);
                SendEvent(NetEvent.PowerUp);
            }
        }
        else if (name == nameof(PlayerData.maxHealth))
        {
            if (orig == PlayerData.instance.GetInt(nameof(PlayerData.maxHealth)) || // If max health hasn't changed
               (orig - PlayerData.instance.GetInt(nameof(PlayerData.maxHealth)) > 1) || // If max health gained is more than 1 (equipping Fragile Heart)
               (orig - PlayerData.instance.GetInt(nameof(PlayerData.maxHealth)) < -1)) // If max health lost is more than 1 (unequipping Fragile Heart)
               return orig;

            // Speedrunner obtained a Mask
            SendEvent(NetEvent.Mask);
            SendEvent(NetEvent.PowerUp);
        }
        else if (name == nameof(PlayerData.MPReserveMax))
        {
            if (orig == PlayerData.instance.GetInt(nameof(PlayerData.MPReserveMax))) return orig;

            // Speedrunner obtained a full Soul Vessel
            SendEvent(NetEvent.SoulVessel);
            SendEvent(NetEvent.PowerUp);
        }
        else if (name == nameof(PlayerData.grubsCollected))
        {
            // Speedrunner freed a grub
            SendEvent(NetEvent.Grub);
        }
        else if (name == nameof(PlayerData.charmSlots))
        {
            // Speedrunner collected a Charm Notch
            SendEvent(NetEvent.CharmNotch);
            SendEvent(NetEvent.PowerUp);
        }
        else if (name
                 is nameof(PlayerData.killsInfectedKnight)
                 or nameof(PlayerData.killsMawlek)
                 or nameof(PlayerData.killsNailBros)
                 or nameof(PlayerData.killsJarCollector)
                 or nameof(PlayerData.killsMegaBeamMiner)
                 or nameof(PlayerData.killsDungDefender)
                 or nameof(PlayerData.killsWhiteDefender)
                 or nameof(PlayerData.killsFalseKnight)
                 or nameof(PlayerData.killsFlukeMother)
                 or nameof(PlayerData.killsLobsterLancer)
                 or nameof(PlayerData.killsNailsage)
                 or nameof(PlayerData.killsGrimm)
                 or nameof(PlayerData.killsNightmareGrimm)
                 or nameof(PlayerData.killsBigFly)
                 or nameof(PlayerData.killsHiveKnight)
                 or nameof(PlayerData.killsHollowKnight)
                 or nameof(PlayerData.killsHornet)
                 or nameof(PlayerData.killsMantisLord)
                 or nameof(PlayerData.killsMegaMossCharger)
                 or nameof(PlayerData.killsMimicSpider)
                 or nameof(PlayerData.killsOblobble)
                 or nameof(PlayerData.killsPaintmaster)
                 or nameof(PlayerData.killsFinalBoss)
                 or nameof(PlayerData.killsMageLord)
                 or nameof(PlayerData.killsMageKnight)
                 or nameof(PlayerData.killsTraitorLord)
                 or nameof(PlayerData.killsMegaJellyfish)
                 or nameof(PlayerData.killsBigBuzzer)
                 or nameof(PlayerData.killsBlackKnight)
                 or nameof(PlayerData.killsZote)
                 or nameof(PlayerData.killsGreyPrince)
                 or nameof(PlayerData.killsHollowKnightPrime)
                 or nameof(PlayerData.killsGhostXero)
                 or nameof(PlayerData.killsGhostAladar)
                 or nameof(PlayerData.killsGhostHu)
                 or nameof(PlayerData.killsGhostMarmu)
                 or nameof(PlayerData.killsGhostNoEyes)
                 or nameof(PlayerData.killsGhostGalien)
                 or nameof(PlayerData.killsGhostMarkoth))
        {
            // Speedrunner defeated a boss
            orig = 1;
            SendEvent(NetEvent.BossKilled);
        }

        return orig;
    }

    private bool ModHooks_OnSetPlayerBoolHook(string name, bool orig)
    {
        // Make sure that the player causing changes is the speedrunner
        if (!IsModuleUsed || TheHuntIsOn.GlobalSaveData.IsHunter)
        {
            return orig;
        }

        var pd = PlayerData.instance;

        // Count the number of movement items before this new boolean is set
        var numMovement = 0;
        if (pd.hasDash) numMovement++;
        if (pd.hasWalljump) numMovement++;
        if (pd.hasSuperDash) numMovement++;
        if (pd.hasDoubleJump) numMovement++;
        if (pd.hasAcidArmour) numMovement++;

        // This method will network the number of movement items we have obtained, based on the number of existing
        // movement items and the newly set boolean from the hook
        void SendConditionalMovement()
        {
            if (numMovement == 0) SendEvent(NetEvent.Movement1);
            if (numMovement == 1) SendEvent(NetEvent.Movement2);
            if (numMovement == 2) SendEvent(NetEvent.Movement3);
            if (numMovement == 3) SendEvent(NetEvent.Movement4);
            if (numMovement == 4) SendEvent(NetEvent.Movement5);
            if (numMovement == 5) SendEvent(NetEvent.Movement6);
        }

        if (orig)
        {
            if (name == nameof(pd.hasDash))
            {
                SendEvent(NetEvent.MothwingCloak);
                SendConditionalMovement();
            }
            else if (name == nameof(pd.hasWalljump))
            {
                SendEvent(NetEvent.MantisClaw);
                SendConditionalMovement();
            }
            else if (name == nameof(pd.hasSuperDash))
            {
                SendEvent(NetEvent.CrystalHeart);
                SendConditionalMovement();
            }
            else if (name == nameof(pd.hasDoubleJump))
            {
                SendEvent(NetEvent.MonarchWings);
                SendConditionalMovement();
            }
            else if (name == nameof(pd.hasAcidArmour))
            {
                SendEvent(NetEvent.IsmasTear);
                SendConditionalMovement();
            }
            else if (name == nameof(pd.hasShadowDash))
            {
                SendEvent(NetEvent.ShadeCloak);
                SendConditionalMovement();
            }
            else if (name == nameof(pd.hasDreamNail))
            {
                SendEvent(NetEvent.DreamNail);
            }
            else if (name == nameof(pd.hasCyclone))
            {
                SendEvent(NetEvent.CycloneSlash);
                SendEvent(NetEvent.PowerUp);
            }
            else if (name == nameof(pd.hasUpwardSlash))
            {
                SendEvent(NetEvent.DashSlash);
                SendEvent(NetEvent.PowerUp);
            }
            else if (name == nameof(pd.hasDashSlash))
            {
                SendEvent(NetEvent.GreatSlash);
                SendEvent(NetEvent.PowerUp);
            }
            else if (name
                     is nameof(pd.lurienDefeated)
                     or nameof(pd.hegemolDefeated)
                     or nameof(pd.monomonDefeated))
            {
                SendEvent(NetEvent.Dreamer);
            }
            else if (name
                     is nameof(pd.openedCrossroads)
                     or nameof(pd.openedDeepnest)
                     or nameof(pd.openedGreenpath)
                     or nameof(pd.openedRuins1)
                     or nameof(pd.openedRuins2)
                     or nameof(pd.openedFungalWastes)
                     or nameof(pd.openedHiddenStation)
                     or nameof(pd.openedRoyalGardens)
                     or nameof(pd.tollBenchCity)
                     or nameof(pd.tollBenchAbyss)
                     or nameof(pd.tollBenchQueensGardens)
                     or nameof(pd.cityLift1))
            {
                SendEvent(NetEvent.Toll);
            }
            else if (name is nameof(pd.cityBridge1) or nameof(pd.cityBridge2))
            {
                SendEvent(NetEvent.LeverHit);
            }
            else if (name is nameof(pd.killedHollowKnight) or nameof(pd.killedFinalBoss)) 
            {
                SendEvent(NetEvent.FinalBossKilled);
            }
            else if (name 
                     is nameof(pd.infectedKnightDreamDefeated)
                     or nameof(pd.falseKnightDreamDefeated)
                     or nameof(pd.mageLordDreamDefeated))
            {
                SendEvent(NetEvent.BossKilled);
            }
            else if (name.StartsWith("gotCharm_"))
            {
                SendEvent(NetEvent.CharmCollected);
            }
        }

        return orig;
    }

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        // Make sure that the player causing changes is the speedrunner
        if (!IsModuleUsed || TheHuntIsOn.GlobalSaveData.IsHunter) return;

        if (self.name.Equals("Stag") && self.Fsm.Name.Equals("Stag Control")) 
        {
            self.InsertCustomAction("Fade", () =>
            {
                SendEvent(NetEvent.Stag);
            }, 0);            

            self.InsertCustomAction("Dirtmouth", () =>
            {
                if (GameManager.instance.sceneName != "Room_Town_Stag_Station")
                    UsedStag(NetEvent.StagDirtmouth);
            }, 2);

            self.InsertCustomAction("Crossroads", () =>
            {
                if (GameManager.instance.sceneName != "Crossroads_47")
                    UsedStag(NetEvent.StagCrossroads);
            }, 2);

            self.InsertCustomAction("Greenpath", () =>
            {
                if (GameManager.instance.sceneName != "Fungus1_16_alt")
                    UsedStag(NetEvent.StagGreenpath);
            }, 2);

            self.InsertCustomAction("Fungal Wastes", () =>
            {
                if (GameManager.instance.sceneName != "Fungus2_02")
                    UsedStag(NetEvent.StagFungalWastes);
            }, 2);

            self.InsertCustomAction("City Storerooms", () =>
            {
                if (GameManager.instance.sceneName != "Ruins1_29")
                    UsedStag(NetEvent.StagCityStorerooms);
            }, 2);

            self.InsertCustomAction("Resting Grounds", () =>
            {
                if (GameManager.instance.sceneName != "RestingGrounds_09")
                    UsedStag(NetEvent.StagRestingGrounds);
            }, 2);

            self.InsertCustomAction("Kings Station", () =>
            {
                if (GameManager.instance.sceneName != "Ruins2_08")
                    UsedStag(NetEvent.StagKingsStation);
            }, 2);

            self.InsertCustomAction("Deepnest", () =>
            {
                if (GameManager.instance.sceneName != "Deepnest_09")
                    UsedStag(NetEvent.StagDeepnest);
            }, 2);

            self.InsertCustomAction("Royal Gardens", () =>
            {
                if (GameManager.instance.sceneName != "Fungus3_40")
                    UsedStag(NetEvent.StagRoyalGardens);
            }, 2);

            self.InsertCustomAction("Hidden Station", () =>
            {
                if (GameManager.instance.sceneName != "Abyss_22")
                    UsedStag(NetEvent.StagHiddenStation);
            }, 2);

            self.InsertCustomAction("Stag Nest", () =>
            {
                if (GameManager.instance.sceneName != "Cliffs_03")
                    UsedStag(NetEvent.StagStagNest);
            }, 2);
        }
        else if (self.name.Equals("Hero Death") && self.Fsm.Name.Equals("Hero Death Anim"))
        {
            self.InsertCustomAction("Start", () =>
            {
                SendEvent(NetEvent.RunnerDeath);
            }, 0);

            self.InsertCustomAction("Anim Start", () =>
            {
                SendEvent(NetEvent.RunnerDreamDeath);
            }, 0);
        }
        else if (self.name.Equals("Inspect Region") && self.Fsm.Name.Equals("Control") &&
            self.gameObject.scene.name is "Room_Tram" or "Room_Tram_RG")
        {
            self.InsertCustomAction("Send Event", () =>
            {
                SendEvent(NetEvent.Tram);
            }, 1);
        }
        else if (self.name.Equals("Toll Gate Machine") && self.Fsm.Name.Equals("Toll Machine"))
        {
            if (self.gameObject.scene.name is "Mines_33" or "Fungus1_31")
            {
                self.InsertCustomAction("Open Gates", () =>
                {
                    SendEvent(NetEvent.Toll);
                }, 2);
            }
        }
        else if (self.name.Equals("Ghost Warrior NPC") && self.Fsm.Name.Equals("Conversation Control"))
        {
            self.InsertCustomAction("Collected", () =>
            {
                SendEvent(NetEvent.DreamWarriorAbsorbed);
            }, 1);

            self.InsertCustomAction("Start Fight", () =>
            {
                SendEvent(NetEvent.DreamWarriorStarted);
            }, 7);
        }
        else if (self.name.Equals("UI List") && self.Fsm.Name.Equals("Confirm Control"))
        {
            var yesState = self.Fsm.GetState("Yes");
            if (yesState == null) return;

            var numActions = yesState.Actions.Length;
            if (numActions > 3) return;

            if (self.gameObject.scene.name == "Ruins1_05b")
            {
                self.InsertCustomAction("Yes", () =>
                {
                    SendEvent(NetEvent.RelicSale);
                }, 0);
            }
            else
            {
                self.InsertCustomAction("Yes", () =>
                {
                    SendEvent(NetEvent.ShopPurchase);
                }, 0);
            }
        }
        else if (self.name.Equals("Nailsmith") && self.Fsm.Name.Equals("Conversation Control"))
        {
            self.InsertCustomAction("Upgrade", () =>
            {
                SendEvent(NetEvent.PowerUp);
                SendEvent(NetEvent.NailUpgrade);
            }, 5);
        }
        else if (self.name.StartsWith("Gate Switch") ||
                 self.name.StartsWith("Toll Gate Switch") ||
                 self.name.StartsWith("Waterways_Crank_Lever") ||
                 self.name.StartsWith("Ruins Lever") ||
                 self.name.StartsWith("Mantis Lever") ||
                 self.name.StartsWith("Mines Lever") ||
                 self.name.StartsWith("WP Lever") ||
                 self.name.StartsWith("White Palace Orb Lever"))
        {
            if (self.Fsm.Name.Equals("Switch Control") || 
                self.Fsm.Name.Equals("toll switch"))
            {
                self.InsertCustomAction("Hit", () =>
                {
                    SendEvent(NetEvent.LeverHit);
                }, 0);
            }
        }
    }

    void UsedStag(NetEvent stagEvent)
    {
        if (TheHuntIsOn.GlobalSaveData.IsHunter) return;
        if (!_clientApi.ClientManager.Players.Any(p => (p.Team != _clientApi.ClientManager.Team) && p.IsInLocalScene)) return;

        SendEvent(stagEvent);
    }

    private void NetManager_OnGrantItemsEvent(NetItem[] netItems)
    {
        // Check if the player is not the speedrunner
        if (!IsModuleUsed || !TheHuntIsOn.GlobalSaveData.IsHunter)
            return;
        
        LogHelper.Write<TheHuntIsOn>("OnGrantItems:");

        var pd = PlayerData.instance;

        foreach (var netItem in netItems)
        {
            LogHelper.Write<TheHuntIsOn>($"  {netItem}");
            switch (netItem)
            {
                case NetItem.VengefulSpirit:
                    PDHelper.FireballLevel = 1;
                    break;
                case NetItem.DesolateDive:
                    PDHelper.QuakeLevel = 1;
                    break;
                case NetItem.HowlingWraiths:
                    PDHelper.ScreamLevel = 1;
                    break;
                case NetItem.ShadeSoul:
                    PDHelper.FireballLevel = 2;
                    break;
                case NetItem.DescendingDark:
                    PDHelper.QuakeLevel = 2;
                    break;
                case NetItem.AbyssShriek:
                    PDHelper.ScreamLevel = 2;
                    break;
                case NetItem.MothwingCloak:
                    PDHelper.HasDash = true;
                    PDHelper.CanDash = true;
                    break;
                case NetItem.MantisClaw:
                    PDHelper.HasWalljump = true;
                    break;
                case NetItem.CrystalHeart:
                    PDHelper.HasSuperDash = true;
                    break;
                case NetItem.MonarchWings:
                    PDHelper.HasDoubleJump = true;
                    break;
                case NetItem.IsmasTear:
                    PDHelper.HasAcidArmour = true;
                    PlayMakerFSM.BroadcastEvent("GET ACID ARMOUR");
                    break;
                case NetItem.ShadeCloak:
                    PDHelper.HasDash = true;
                    PDHelper.CanDash = true;
                    PDHelper.HasShadowDash = true;
                    break;
                case NetItem.DreamNail:
                    PDHelper.HasDreamNail = true;
                    break;
                case NetItem.CycloneSlash:
                    PDHelper.HasNailArt = true;
                    PDHelper.HasCyclone = true;
                    break;
                case NetItem.DashSlash:
                    PDHelper.HasNailArt = true;
                    PDHelper.HasUpwardSlash = true;
                    break;
                case NetItem.GreatSlash:
                    PDHelper.HasNailArt = true;
                    PDHelper.HasDashSlash = true;
                    break;
                case NetItem.MaskShard:
                    PDHelper.HeartPieces += 1;

                    if (PDHelper.HeartPieces == 4)
                    {
                        if (pd.maxHealthBase < 9)
                        {
                            HeroController.instance.MaxHealth();
                            HeroController.instance.AddToMaxHealth(1);
                            PlayMakerFSM.BroadcastEvent("MAX HP UP");
                        }

                        PDHelper.HeartPieces = 0;
                    }

                    break;
                case NetItem.Mask:
                    if (pd.maxHealthBase < 9)
                    {
                        HeroController.instance.MaxHealth();
                        HeroController.instance.AddToMaxHealth(1);
                        PlayMakerFSM.BroadcastEvent("MAX HP UP");
                    }

                    break;
                case NetItem.SoulVessel:
                    if (pd.MPReserveMax < 99)
                    {
                        HeroController.instance.AddToMaxMPReserve(33);
                        PlayMakerFSM.BroadcastEvent("NEW SOUL ORB");
                    }

                    break;
                case NetItem.NailUpgrade:
                    // Find the first nail upgrade that the player does not have yet
                    // In other words find the first damage value from the array for which the player's nail damage
                    // is lower and set it to that value
                    var damages = new[] { 9, 13, 17, 21 };
                    foreach (var damage in damages)
                    {
                        if (pd.nailDamage < damage)
                        {
                            pd.honedNail = true;
                            pd.nailDamage = damage;
                            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
                            pd.nailSmithUpgrades += 1;
                            break;
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    #endregion

    #region Methods

    private void SendEvent(NetEvent netEvent)
    {
        LogHelper.Write<TheHuntIsOn>($"Sending triggered event: {netEvent}");
        HuntClientAddon.NetManager.SendEvent(netEvent);
    }

    internal override void Enable()
    {
        if (!AreAddonsLoaded) return;

        ModHooks.SetPlayerIntHook += ModHooks_OnSetPlayerIntHook;
        ModHooks.SetPlayerBoolHook += ModHooks_OnSetPlayerBoolHook;
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;

        HuntClientAddon.NetManager.GrantItemsEvent += NetManager_OnGrantItemsEvent;
    }

    internal override void Disable()
    {
        if (!AreAddonsLoaded) return;

        ModHooks.SetPlayerIntHook -= ModHooks_OnSetPlayerIntHook;
        ModHooks.SetPlayerBoolHook -= ModHooks_OnSetPlayerBoolHook;
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;

        HuntClientAddon.NetManager.GrantItemsEvent -= NetManager_OnGrantItemsEvent;
    }

    #endregion
}