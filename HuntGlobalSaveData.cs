using System.Collections.Generic;
using TheHuntIsOn.Modules.PauseModule;
using TheHuntIsOn.Modules.PauseTimerModule;

namespace TheHuntIsOn;

public class HuntGlobalSaveData
{
	#region Properties

	public bool IsHunter { get; set; }

	public Dictionary<string, ModuleAffection> AffectionTable { get; set; } = new();

	public float FocusSpeed { get; set; } = 1.0f;

	public int FocusCost { get; set; } = 33;

	public int SpellCost { get; set; } = 33;

	public PauseTimerPosition PauseTimerPosition { get; set; } = PauseTimerPosition.BottomCenter;

	public PauseTimerSize PauseTimerSize { get; set; } = PauseTimerSize.Normal;

    #endregion
}
