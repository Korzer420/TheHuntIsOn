using IL.InControl.NativeDeviceProfiles;
using System.Collections.Generic;
using UnityEngine;

namespace TheHuntIsOn.Modules;

internal class HelperPlatformModule : Module
{
    #region Properties

    public override string MenuDescription => "Places platforms to allow reaching certain spots without skips.";

    public static bool HasDash => PlayerData.instance.GetBool(nameof(PlayerData.hasDash));

    public static bool HasClaw => PlayerData.instance.GetBool(nameof(PlayerData.hasWalljump));

    public static bool HasWings => PlayerData.instance.GetBool(nameof(PlayerData.hasDoubleJump));

    public static bool HasCrystalHeart => PlayerData.instance.GetBool(nameof(PlayerData.hasSuperDash));

    #endregion

    #region Eventhandler

    private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
    {
        if (!IsModuleUsed)
            return;
        switch (newScene.name)
        {
            // Broken Elevator
            case "Abyss_01" when !HasDash:
                MakePlatform(15.50f, 152.75f);
                break;
            // Below Hidden Station Tram
            case "Abyss_04" when !HasWings:
                MakePlatform(84.0f, 11.42f);
                MakePlatform(84.0f, 16.31f);
                break;
            // Broken Vessel Skip
            case "Abyss_18" when !HasWings && !HasCrystalHeart:
                MakePlatform(35.4f, 6.1f);
                MakePlatform(31.25f, 9.1f);
                break;
            // Monarch Wings Room
            case "Abyss_21" when !HasWings:
                MakePlatform(122.90f, 223.60f);
                MakePlatform(130.10f, 231.60f);
                MakePlatform(137.81f, 241.60f);
                break;
            // Gorb Room
            case "Cliffs_02":
                MakePlatform(39.03f, 36.80f);
                MakePlatform(33.04f, 40.90f);
                MakePlatform(29.08f, 44.62f);
                break;
            // Ancestral Mound
            case "Crossroads_ShamanTemple" when !HasWings:
                MakePlatform(14.43f, 55.61f);
                break;
            // Gruz Mother Room
            case "Crossroads_04":
                if ((HasWings && HasClaw) || (HasClaw && HasCrystalHeart)) break;
                MakePlatform(151.9073f, 15.0591f);
                MakePlatform(148.2994f, 19.5866f);
                if (!HasDash && !HasClaw && !HasCrystalHeart && !HasWings) MakePlatform(139.7048f, 6.13f);
                break;
            // Mask Shard Crossroads besides hot spring
            case "Crossroads_13" when !HasClaw && !HasWings:
                MakePlatform(21.92f, 15.26f);
                break;
            // Entrance to Fungal Wastes
            case "Crossroads_18" when !HasDash && !HasWings:
                MakePlatform(27.30f, 37.00f);
                break;
            // Hornet 2 Aspid Skip
            case "Deepnest_East_11" when !HasWings:
                MakePlatform(105.00f, 97.36f);
                MakePlatform(103.00f, 106.51f);
                MakePlatform(92.2f, 113.58f);
                break;
            // Failed Tramway
            case "Deepnest_26b" when !HasDash && !HasWings:
                MakePlatform(115.00f, 14.00f); // Path to Lever
                break;
            // Room Below QG Toll Bench (beside Mask Maker)
            case "Deepnest_42" when !HasWings:
                MakePlatform(19.50f, 129.00f);
                break;
            // Fireball Skip Room Greenpath
            case "Fungus1_02" when !HasDash && !HasClaw && !HasWings:
                MakePlatform(8.03f, 57.50f);
                break;
            // Second Fireball Skip Room Greenpath
            case "Fungus1_03" when !HasDash && !HasClaw && !HasWings:
                MakePlatform(49.34f, 19.25f);
                break;
            // Hornet Room
            case "Fungus1_04" when !HasClaw && !HasWings:
                MakePlatform(38.20f, 32.00f);
                break;
            // Above Fog Canyon
            case "Fungus1_11" when !HasDash && !HasClaw && !HasWings:
                MakePlatform(5.20f, 29.30f);
                break;
            // Queen's Station
            case "Fungus2_01" when !HasDash && !HasClaw && !HasWings:
                MakePlatform(32.00f, 37.50f);
                break;
            // Fungal Entrance (outside Leg Eater)
            case "Fungus2_06" when !HasDash && !HasClaw && !HasWings:
                MakePlatform(20.69f, 56.00f);
                break;
            // Shrumal Warrior Room
            case "Fungus2_07" when !HasDash && !HasClaw && !HasWings:
                MakePlatform(64.48f, 7.50f);
                break;
            // Explosion Pogo
            case "Fungus2_11" when !HasClaw && !HasWings:
                MakePlatform(4.26f, 8.07f);
                break;
            // Mantis Village Entrance
            case "Fungus2_12" when !HasDash && !HasClaw && !HasWings:
                MakePlatform(118.84f, 6.82f);
                break;
            // Bretta Bench
            case "Fungus2_13" when !HasClaw && !HasWings:
                MakePlatform(25.50f, 61.50f);
                MakePlatform(35.00f, 70.00f);
                if (!HasDash)
                {
                    MakePlatform(23.50f, 27.50f);
                    MakePlatform(13.95f, 20.00f);
                    MakePlatform(7.20f, 3.86f);
                }
                break;
            // Mantis Village
            case "Fungus2_14" when !HasClaw && !HasWings:
                MakePlatform(16.30f, 24.04f);
                MakePlatform(10.96f, 29.37f);
                MakePlatform(47.92f, 3.15f);
                MakePlatform(55.60f, 3.15f);
                MakePlatform(111.96f, 9.28f);
                if (!HasDash)
                {
                    MakePlatform(53.10f, 11.00f);
                    MakePlatform(133.00f, 9.28f);
                }
                break;
            // Mantis Lords
            case "Fungus2_15" when !HasWings && HasClaw:
                MakePlatform(14.30f, 10.61f); // to reach Lifeblood Cocoon
                break;
            /* 
            // Journal Room Beside Epogo
            case "Fungus2_17" when !HasClaw && !HasWings:
                MakePlatform(24.00f, 6.75f);
                MakePlatform(24.00f, 10.75f);
                MakePlatform(24.00f, 14.75f);
                MakePlatform(14.86f, 22.75f);
                MakePlatform(14.86f, 26.75f);
                MakePlatform(19.00f, 34.75f);
                break; */
            // Cornifer Room
            case "Fungus2_18" when !HasDash && !HasClaw && !HasWings:
                MakePlatform(76.90f, 6.59f);
                break;
            // Lever Room beside Blue Lake
            case "RestingGrounds_06" when !HasDash && !HasClaw && !HasWings:
                MakePlatform(25.5f, 17.0f);
                MakePlatform(17.6f, 22.0f);
                break;
            // Outside City Storerooms Stag
            case "Ruins1_28" when !HasClaw && !HasWings:
                MakePlatform(42.67f, 5.43f);
                break;
            // Spire Bottom
            case "Ruins2_01_b" when !HasClaw && !HasWings:
                MakePlatform(30.56f, 31.59f);
                break;
            // King's Station
            case "Ruins2_06" when !HasClaw && !HasWings:
                MakePlatform(15.94f, 21.22f);
                break;
            // Soul Master
            case "Ruins1_24":
            case "Ruins1_24_boss_defeated":
                MakePlatform(31.63f, 12.21f);
                MakePlatform(31.63f, 16.01f);
                MakePlatform(31.63f, 19.81f);
                MakePlatform(27.63f, 23.61f);
                break;
            // Soul Tyrant Dream
            case "Dream_02_Mage_Lord":
                MakePlatform(10.10f, 12.21f);
                MakePlatform(10.10f, 16.01f);
                MakePlatform(10.10f, 19.81f);
                MakePlatform(14.10f, 23.61f);
                break;
            default:
                break;
        }
    }

    #endregion

    #region Methods
    private void MakePlatform(float xpos, float ypos)
    {
        GameObject currentGameObject;

        currentGameObject = GameObject.Instantiate(ShadeSkipModule.PlatformPrefab, new(xpos, ypos, 0), Quaternion.identity);
        currentGameObject.SetActive(true);
    }

    internal override void Enable()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    internal override void Disable()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    #endregion
}
