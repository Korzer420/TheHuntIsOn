using KorzUtils.Helper;
using MonoMod.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheHuntIsOn.Modules.HealthModules;

internal class MaskModule : Module
{
    #region Properties

    public override string MenuDescription => "Disables heal when picking up a new mask.";

    #endregion

    #region Eventhandler

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed && self.FsmName == "Heart Container UI")
            self.GetState("Heal 1 HP").AdjustTransition("FINISHED", "Max Up");
        orig(self);
    }

    private void SendEventByName_OnEnter(On.HutongGames.PlayMaker.Actions.SendEventByName.orig_OnEnter orig, HutongGames.PlayMaker.Actions.SendEventByName self)
    {
        if (IsModuleUsed && self.IsCorrectContext("Heart Container UI", "Heart Container UI*", "Max Up"))
        {
            int currentSoul = PlayerData.instance.GetInt(nameof(PlayerData.MPCharge));
            // The soul is depleted on reappearing so we restore that.
            CoroutineHelper.WaitForHero(x =>
            {
                GameCameras.instance.hudCanvas.gameObject.SetActive(false);
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
                CoroutineHelper.WaitFrames(HeroController.instance.AddMPCharge, x, true);
            }, currentSoul, true);
        }
        orig(self);
    }

    private void pd_AddToMaxHealth(MonoMod.Cil.ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.Goto(0);
        if (cursor.TryGotoNext(MoveType.After,
            x => x.MatchCallvirt<PlayerData>("SetIntSwappedArgs"),
            x => x.MatchLdarg(0),
            x => x.MatchLdarg(0),
            x => x.MatchLdstr("maxHealth"),
            x => x.MatchCallvirt<PlayerData>("GetInt")))
            cursor.EmitDelegate<Func<int, int>>(x => IsModuleUsed ? PlayerData.instance.GetInt(nameof(PlayerData.health)) : x);
        else
            LogHelper.Write<TheHuntIsOn>("Failed to prevent mask heal.", KorzUtils.Enums.LogType.Error);
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        IL.PlayerData.AddToMaxHealth += pd_AddToMaxHealth;
        On.HutongGames.PlayMaker.Actions.SendEventByName.OnEnter += SendEventByName_OnEnter;
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
    }

    internal override void Disable()
    {
        IL.PlayerData.AddToMaxHealth -= pd_AddToMaxHealth;
        On.HutongGames.PlayMaker.Actions.SendEventByName.OnEnter -= SendEventByName_OnEnter;
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
    }

    #endregion
}
