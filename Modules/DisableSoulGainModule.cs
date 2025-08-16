using Modding;
using MonoMod.Cil;
using System;

namespace TheHuntIsOn.Modules;

internal class DisableSoulGainModule : Module
{
    #region Properties

    public override string MenuDescription => "Causes all hits to return 0 soul.";

    #endregion

    #region Eventhandler

    private int SoulGainHook(int amount)
    {
        if (IsModuleUsed)
        {
            amount = 0;
        }

        return amount;
    }

    private void DreamNailSoulAdjuster(ILContext il)
    {
        ILCursor cursor = new ILCursor(il).Goto(0);

        // Regular Soul
        cursor.TryGotoNext(i => i.MatchLdcI4(33));
        cursor.GotoNext();
        cursor.EmitDelegate<Func<int, int>>(soul => IsModuleUsed ? 0 : 33);

        // Charm Soul
        cursor.TryGotoNext(i => i.MatchLdcI4(66));
        cursor.GotoNext();
        cursor.EmitDelegate<Func<int, int>>(soul => IsModuleUsed ? 0 : 66);
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        ModHooks.SoulGainHook += SoulGainHook;
        IL.EnemyDreamnailReaction.RecieveDreamImpact += DreamNailSoulAdjuster;
    }

    internal override void Disable()
    {
        ModHooks.SoulGainHook -= SoulGainHook;
        IL.EnemyDreamnailReaction.RecieveDreamImpact -= DreamNailSoulAdjuster;
    }

    #endregion
}
