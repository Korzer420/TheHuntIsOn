using System.Collections.Generic;

namespace TheHuntIsOn;

public class HuntGlobalSaveData
{
	#region Properties

	public bool IsHunter { get; set; }

	public Dictionary<string, ModuleAffection> AffectionTable { get; set; } = new();

	public float FocusSpeed { get; set; } = 1.0f;

	public int FocusCost { get; set; } = 33;

	public int SpellCost { get; set; } = 33;

	public bool DisableEnemies { get; set; }

	public bool InvincibleBosses { get; set; }

	public bool DreamBossAccess { get; set; }

    #endregion
}
