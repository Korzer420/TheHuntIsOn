using System.Text;

namespace TheHuntIsOn;

internal abstract class Module
{
    #region Member

    private bool _active;

    #endregion

    #region Properties

    public ModuleAffection Affection { get; set; }

    public bool IsModuleUsed => Affection == ModuleAffection.All || (TheHuntIsOn.SaveData.IsHunter && Affection == ModuleAffection.OnlyHunter)
        || (!TheHuntIsOn.SaveData.IsHunter && Affection == ModuleAffection.OnlySpeedrunner);

    public abstract string MenuDescription { get; }

    public static bool DisableEnemies { get; set; }
    public static bool InvincibleBosses { get; set; }
    public static bool DreamBossAccess { get; set; }

    #endregion

    #region Methods

    internal void Load()
    {
        if (_active)
            return;
        Enable();
        _active = true;
    }

    internal void Unload()
    {
        if (!_active)
            return;
        Disable();
        _active = false;
    }

    internal abstract void Enable();

    internal abstract void Disable();

    internal string GetModuleName()
    {
        string moduleName = GetType().Name;
        StringBuilder builder = new();
        foreach (char letter in moduleName)
        {
            if (char.IsUpper(letter))
                builder.Append(" ");
            builder.Append(letter);
        }
        return builder.ToString();
    }

    #endregion
}
