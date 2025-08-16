using KorzUtils.Enums;
using KorzUtils.Helper;
using Modding;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;

namespace TheHuntIsOn;

internal static class HealthControl
{
    #region Members

    private static ILHook _hook;

    #endregion

    #region Constructors

    static HealthControl()
    {
        ModHooks.AfterTakeDamageHook += ModHooks_AfterTakeDamageHook;
        IL.PlayerData.MaxHealth += pd_MaxHealth;
        ModHooks.BeforeAddHealthHook += ModHooks_BeforeAddHealthHook;
        On.HeroController.MaxHealth += HeroController_MaxHealth;
        ModHooks.BlueHealthHook += ModHooks_BlueHealthHook;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Get or sets the already lost lifeblood from Lifeblood heart.
    /// </summary>
    public static int LifebloodHeart { get; set; } = 0;

    /// <summary>
    /// Gets or sets the already lost lifeblood from Lifeblood core.
    /// </summary>
    public static int LifebloodCoreHealth { get; set; } = 0;

    /// <summary>
    /// Gets or sets the joni health, if jonis blessing is equipped.
    /// </summary>
    public static int JoniUsedHealth { get; set; }

    /// <summary>
    /// Gets or sets the current health, if joni is unequipped. Used to transition between the two.
    /// </summary>
    public static int CurrentHealth { get; set; }

    /// <summary>
    /// Gets or sets how many masks of the fragile heart have been removed already. (Prevents re equipping from healing)
    /// </summary>
    public static int FragileHeartHealth { get; set; }

    public static bool BlockHeal { get; set; }

    #endregion

    #region Eventhandler

    private static int ModHooks_AfterTakeDamageHook(int hazardType, int damageAmount)
    {
        int currentHealth = CharmHelper.EquippedCharm(CharmRef.JonisBlessing)
            ? PlayerData.instance.GetInt("joniHealthBlue")
            : PlayerData.instance.GetInt("health");

        int lifeblood = PlayerData.instance.GetInt("healthBlue");
        int actualDamage = PlayerData.instance.GetBool(nameof(PlayerData.overcharmed)) ? damageAmount * 2 : damageAmount;
        // If the attack is deadly, we don't need to calculate anything.
        if (actualDamage >= currentHealth + lifeblood)
            return damageAmount;

        // To determine which of the charms should lose a "point" we check for the index.
        int lifebloodHeartIndex = PlayerData.instance.equippedCharms.IndexOf((int)CharmRef.LifebloodHeart);
        int lifebloodCoreIndex = PlayerData.instance.equippedCharms.IndexOf((int)CharmRef.LifebloodCore);
        for (int i = 0; i < actualDamage; i++)
            if (CharmHelper.EquippedCharm(CharmRef.JonisBlessing))
                JoniUsedHealth++;
            else if (lifeblood > 0)
            {
                if (lifebloodCoreIndex > lifebloodHeartIndex && LifebloodCoreHealth < 4)
                    LifebloodCoreHealth++;
                else if (LifebloodHeart < 2)
                    LifebloodHeart++;
                else if (LifebloodCoreHealth < 4)
                    LifebloodCoreHealth++;
                lifeblood--;
            }
            else
            {
                if (CharmHelper.EquippedCharm(CharmRef.FragileHeart) && FragileHeartHealth < 2)
                    FragileHeartHealth++;
                else
                    CurrentHealth--;
            }
        return damageAmount;
    }

    private static void pd_MaxHealth(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.Goto(0);

        cursor.GotoNext(MoveType.After,
            x => x.MatchCallvirt<PlayerData>("get_CurrentMaxHealth"));
        cursor.EmitDelegate<Func<int, int>>(x =>
        {
            if (BlockHeal)
            {
                LogHelper.Write("Current health: " + CurrentHealth + "; CurrentMaxHealth: " + x);
                int currentHealth = CurrentHealth;
                if (CharmHelper.EquippedCharm(CharmRef.FragileHeart) && !CharmHelper.EquippedCharm(CharmRef.JonisBlessing))
                    currentHealth += 2 - FragileHeartHealth;
                return Math.Min(x, currentHealth);
            }
            else
                Reset(x);
            if (_hook == null)
                SetJoniHook();
            return x;
        });
    }

    private static int ModHooks_BeforeAddHealthHook(int healthToRestore)
    {
        // In case hiveblood restores Joni health
        if (CharmHelper.EquippedCharm(CharmRef.JonisBlessing))
            JoniUsedHealth = Math.Max(0, JoniUsedHealth - healthToRestore);
        else
        {
            int currentHealth = PlayerData.instance.GetInt(nameof(PlayerData.health));
            int maxHealth = PlayerData.instance.GetInt(nameof(PlayerData.maxHealth));
            if (CharmHelper.EquippedCharm(CharmRef.FragileHeart))
            {
                for (int i = 0; i < healthToRestore; i++)
                {
                    if (currentHealth + 2 >= maxHealth)
                        FragileHeartHealth = Math.Max(0, FragileHeartHealth - 1);
                    else
                        CurrentHealth = Math.Min(maxHealth, CurrentHealth + 1);
                    currentHealth++;
                }
            }
            else
                CurrentHealth = Math.Min(maxHealth, CurrentHealth + healthToRestore);
        }
        return healthToRestore;
    }

    private static void HeroController_MaxHealth(On.HeroController.orig_MaxHealth orig, HeroController self)
    {
        if (BlockHeal)
        {
            PlayerData.instance.MaxHealth();
            return;
        }
        orig(self);
    }

    private static int ModHooks_BlueHealthHook()
    {
        if (BlockHeal)
            return 0;
        int toTake = 0;
        if (CharmHelper.EquippedCharm(CharmRef.LifebloodCore))
            toTake += LifebloodCoreHealth;
        if (CharmHelper.EquippedCharm(CharmRef.LifebloodHeart))
            toTake += LifebloodHeart;
        return toTake * -1;
    }

    #endregion

    #region Methods

    internal static void Reset(int maxHealth)
    {
        LifebloodCoreHealth = 0;
        LifebloodHeart = 0;
        JoniUsedHealth = 0;
        FragileHeartHealth = 0;
        CurrentHealth = maxHealth == 1
        ? PlayerData.instance.GetInt(nameof(PlayerData.maxHealthBase))
        : (CharmHelper.EquippedCharm(CharmRef.FragileHeart)
            ? maxHealth - 2
            : maxHealth);
    }

    private static void SetJoniHook()
    {
        _hook = new(typeof(HeroController).GetMethod("orig_CharmUpdate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public),
            new ILContext.Manipulator(x =>
            {
                ILCursor cursor = new(x);
                cursor.Goto(0);
                cursor.GotoNext(MoveType.After,
                    x => x.MatchMul());
                cursor.EmitDelegate<Func<float, float>>(x => BlockHeal ? Math.Max(1, x - JoniUsedHealth) : x);
            }));
    }

    #endregion
}
