using KorzUtils.Data;
using KorzUtils.Helper;
using Modding;
using System;
using UnityEngine;

namespace TheHuntIsOn.Modules;

internal class ShadeSkipModule : Module
{
    #region Properties

    public override string MenuDescription => "Allows navigation through skip locations requiring the shade.";

    public static GameObject PlatformPrefab { get; set; }

    public static int ShadeHealth => (PlayerData.instance.GetInt(nameof(PlayerData.nailSmithUpgrades)) * 4 + 5) *
                                     (PlayerData.instance.GetInt(nameof(PlayerData.maxHealthBase)) / 2);

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
            // Gruz Mother Room
            case "Crossroads_04":
                if ((HasWings && HasClaw) || (HasClaw && HasCrystalHeart)) break;
                MakePlatform(151.9073f, 15.0591f);
                MakePlatform(148.2994f, 19.5866f);
                if (!HasDash && !HasClaw && !HasCrystalHeart && !HasWings) MakePlatform(139.7048f, 6.13f);
                break;
            // Ancient Basin Toll Bench
            case "Abyss_18" when !HasWings && !HasCrystalHeart:
                if (PlayerData.instance.GetBool(nameof(PlayerData.hasWalljump)) && 
                    PlayerData.instance.GetInt(nameof(PlayerData.fireballLevel)) > 0 &&
                    ShadeHealth >= 20)
                {
                    MakePlatform(35.4f, 6.1f);
                    MakePlatform(31.25f, 9.1f);
                }
                break;
            // Ancient Basin Below Tram
            case "Abyss_04" when !HasWings:
                if (PlayerData.instance.GetInt(nameof(PlayerData.fireballLevel)) > 0 && 
                    PlayerData.instance.GetBool(nameof(PlayerData.hasWalljump)))
                {
                    MakePlatform(84.0f, 11.42f);
                    MakePlatform(84.0f, 16.31f);
                }
                break;
            // Baldur Shell
            case "Fungus1_28" when !HasDash && !HasWings && !HasClaw && !HasCrystalHeart:
                MakePlatform(60.00f, 28.25f);
                break;
            // Shrumal Warrior Room
            case "Fungus2_07" when !HasDash && !HasClaw && !HasWings:
                MakePlatform(64.48f, 7.50f);
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

        currentGameObject = GameObject.Instantiate(PlatformPrefab, new(xpos, ypos, 0), Quaternion.identity);
        currentGameObject.SetActive(true);
    }

    internal override void Enable()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    internal override void Disable()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    #endregion
}
