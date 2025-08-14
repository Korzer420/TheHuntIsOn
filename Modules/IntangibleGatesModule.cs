using KorzUtils.Helper;
using TheHuntIsOn.Components;

namespace TheHuntIsOn.Modules;

internal class IntangibleGatesModule : Module
{
    #region Properties

    public override string MenuDescription => "Allows the player to walk through gates.";

    #endregion

    #region Methods

    internal override void Enable()
    {
        CoroutineHelper.WaitForHero(() =>
            {
                if (HeroController.instance.GetComponent<IgnoreGates>() == null)
                    HeroController.instance.gameObject.AddComponent<IgnoreGates>();
            }, true);
    }

    internal override void Disable(){}

    #endregion
}
