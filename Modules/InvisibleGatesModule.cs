using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheHuntIsOn.Modules;

internal class InvisibleGatesModule : Module
{
    #region Properties

    public override string MenuDescription => "Makes all gates invisible.";

    #endregion

    #region Eventhandler

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed)
        {
            if (self.gameObject.tag == "Battle Gate" || 
                self.gameObject.tag.Contains("Dream Gate") ||
                self.transform.parent?.name == "Hornet Saver" || 
                self.gameObject.name == "Wall Saver" || 
                self.gameObject.name == "Enemy Saver" ||
                self.transform.parent?.name == "infected_door" || 
                self.transform.name == "Colliders" && (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Deepnest_East_Hornet" && self.transform.parent?.name == "Battle Scene") ||
                self.transform.name == "Colliders" && (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Fungus2_15" && self.transform.parent?.name == "mantis_cage_down"))
            {
                tk2dSprite component = self.gameObject.GetComponent<tk2dSprite>();
                Color color = component.color;

                color.a = 0f;
                component.color = color;
            }
        }

        orig(self);
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene newScene)
    {
        if (IsModuleUsed)
            if (newScene.name == "Room_Final_Boss_Core")
                GameObject.Destroy(GameObject.Find("Gate"));
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    internal override void Disable()
    {
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    #endregion
}
