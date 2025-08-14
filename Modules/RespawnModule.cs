using KorzUtils.Data;
using KorzUtils.Helper;
using Modding;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Reflection;

namespace TheHuntIsOn.Modules;

internal class RespawnModule : Module
{
    #region Members

    ILHook _hook;

    #endregion

    #region Properties

    public override string MenuDescription => "Force the player to ALWAYS respawn in Kings Pass.";

    #endregion

    #region Eventhandler

    private void ModHooks_SceneChanged(string obj) => OnDeath();

    private void OnDeath()
    {
        if (!IsModuleUsed)
            return;
        var pd = PlayerData.instance;

        pd.respawnScene = "Tutorial_01";
        pd.mapZone = GlobalEnums.MapZone.KINGS_PASS;
        pd.respawnMarkerName = "Death Respawn Marker";
        pd.respawnType = 0;
        pd.respawnFacingRight = true;
    }

    private void HeroController_Die(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.Goto(0);

        if (cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdfld<HeroController>("gm"),
            x => x.MatchCallvirt<GameManager>("GetCurrentMapZone")))
            cursor.EmitDelegate<Func<string, string>>(x => IsModuleUsed ? "Emptiness" : x);
    }

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed && self.FsmName == "Hero Death Anim" && self.GetState("Should Die?") == null)
        {
            self.AddState("Should Die?", () => self.SendEvent(IsModuleUsed ? "FINISHED" : "DREAM"),
                FsmTransitionData.FromTargetState("Anim Start").WithEventName("DREAM"),
                FsmTransitionData.FromTargetState("Break Glass HP").WithEventName("FINISHED"));
            self.GetState("Map Zone").AdjustTransition("DREAM", "Should Die?");
        }
        orig(self);
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        ModHooks.BeforePlayerDeadHook += OnDeath;
        ModHooks.SceneChanged += ModHooks_SceneChanged;
        ModHooks.AfterPlayerDeadHook += OnDeath;
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        _hook = new(typeof(HeroController).GetMethod("Die", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetStateMachineTarget(), HeroController_Die);
    }

    internal override void Disable()
    {
        ModHooks.BeforePlayerDeadHook -= OnDeath;
        ModHooks.SceneChanged -= ModHooks_SceneChanged;
        ModHooks.AfterPlayerDeadHook -= OnDeath;
        _hook?.Dispose();
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
    }

    #endregion
}
