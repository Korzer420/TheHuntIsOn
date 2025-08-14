using GlobalEnums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheHuntIsOn.Modules;

/// <summary>
/// Blocker code taken from https://github.com/SFGrenade/FireIsSmolBrain (from SFG)
/// </summary>
internal class ArenaModule : Module
{
    #region Properties

    public override string MenuDescription => "Creates invisible barriers around some boss arenas.";

    #endregion

    #region Eventhandler

    private void SceneManager_activeSceneChanged(Scene arg0, Scene newScene)
    {
        if (!IsModuleUsed)
            return;
        switch (newScene.name)
        {
            case "Crossroads_04":
                MakeBlocker(newScene, new Vector3(94.5f, 19.5f), new Vector2(17, 11));
                break;
            case "Crossroads_09":
                MakeBlocker(newScene, new Vector3(61.5f, 13.5f), new Vector2(25, 21));
                break;
            case "Crossroads_10":
                MakeBlocker(newScene, new Vector3(28.5f, 35), new Vector2(35, 18));
                break;
            case "Ruins1_24":
                MakeBlocker(newScene, new Vector3(21, 32.5f), new Vector2(32, 9));
                MakeBlocker(newScene, new Vector3(26.5f, 42.5f), new Vector2(43, 11));
                MakeBlocker(newScene, new Vector3(34.5f, 53), new Vector2(49, 10));
                MakeBlocker(newScene, new Vector3(25, 17), new Vector2(34, 16));
                break;
            case "Ruins1_31b":
                MakeBlocker(newScene, new Vector3(30.5f, 45), new Vector2(27, 14));
                break;
            case "Ruins2_03":
                MakeBlocker(newScene, new Vector3(43.5f, 80.5f), new Vector2(39, 23));
                break;
            case "Ruins2_11":
                MakeBlocker(newScene, new Vector3(54.5f, 100), new Vector2(31, 12));
                break;
            case "Fungus1_04":
                MakeBlocker(newScene, new Vector3(26.5f, 36.5f), new Vector2(23, 19));
                break;
            case "Fungus1_20_v02":
                MakeBlocker(newScene, new Vector3(47.5f, 17.5f), new Vector2(15, 11));
                break;
            case "Fungus1_29":
                MakeBlocker(newScene, new Vector3(49.5f, 9.5f), new Vector2(43, 7));
                break;
            case "Fungus2_15":
                MakeBlocker(newScene, new Vector3(30, 10), new Vector2(26, 8));
                break;
            case "Fungus3_23":
                MakeBlocker(newScene, new Vector3(40, 32), new Vector2(40, 8));
                break;
            case "Fungus3_archive_02":
                MakeBlocker(newScene, new Vector3(53, 120), new Vector2(38, 36));
                break;
            case "Mines_18":
                MakeBlocker(newScene, new Vector3(30, 13), new Vector2(20, 6));
                break;
            case "Mines_32":
                MakeBlocker(newScene, new Vector3(30, 17.5f), new Vector2(22, 15));
                break;
            case "Deepnest_32":
                MakeBlocker(newScene, new Vector3(94, 11), new Vector2(50, 18));
                break;
            case "Deepnest_East_Hornet":
                MakeBlocker(newScene, new Vector3(27, 33), new Vector2(24, 14));
                break;
            case "Abyss_19":
                MakeBlocker(newScene, new Vector3(27, 34), new Vector2(24, 15));
                break;
            case "Waterways_05":
                MakeBlocker(newScene, new Vector3(76, 13), new Vector2(34, 16));
                break;
            case "Waterways_12":
                MakeBlocker(newScene, new Vector3(20, 17), new Vector2(36, 30));
                break;
            case "Hive_05":
                MakeBlocker(newScene, new Vector3(69, 33), new Vector2(24, 14));
                break;
            case "Grimm_Main_Tent":
                MakeBlocker(newScene, new Vector3(87, 15), new Vector2(44, 20));
                break;
            default:
                break;
        }
    }

    #endregion

    #region Methods

    internal override void Enable() 
    { 
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    internal override void Disable() 
    { 
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }
    
    private void MakeBlocker(Scene sc, Vector3 p, Vector2 si)
    {
        if (!IsModuleUsed)
            return;
        var blocker = new GameObject("ArenaBlocker", typeof(BoxCollider2D));
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(blocker, sc);
        blocker.layer = (int)PhysLayers.TERRAIN;
        blocker.transform.position = p;
        blocker.GetComponent<BoxCollider2D>().size = si;
    }

    #endregion
}
