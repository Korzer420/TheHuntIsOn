namespace TheHuntIsOn.Modules.PauseTimerModule;

public enum PauseTimerSize
{
    Normal,
    Small,
    Large,
}

public static class PauseTimerSizeExtensions
{
    public static float FontScale(this PauseTimerSize size) => size switch { PauseTimerSize.Normal => 0.3f, PauseTimerSize.Small => 0.2f, PauseTimerSize.Large => 0.4f };

    public static float Spacing(this PauseTimerSize size) => size switch { PauseTimerSize.Normal => 0.95f, PauseTimerSize.Small => 0.6f, PauseTimerSize.Large => 1.3f };
}
