using UnityEngine;

namespace TheHuntIsOn.Modules.PauseTimerModule;

public enum PauseTimerPosition
{
    BottomLeft,
    BottomCenter,
    BottomRight,
    CenterLeft,
    CenterRight,
    TopCenter,
    TopRight,
    BelowUI,
}

public record PauseTimerSpacingParameters(PauseTimerPosition position, Vector2 anchorPos, Vector2 forwardSpace, Vector2 reverseSpace)
{
    public Vector2 GetPosition(int i, int count, float spacing, float scale, Bounds localBounds)
    {
        Vector2 pos = anchorPos + spacing * (forwardSpace * i + reverseSpace * (count - 1 - i));
        Vector2 pivot = new(
            position.IsLeft() ? -localBounds.min.x : (position.IsRight() ? -localBounds.max.x : -localBounds.center.x),
            position.IsBottom() ? -localBounds.min.y : (position.IsTop() ? -localBounds.max.y : -localBounds.center.y));
        return pos + pivot * scale;
    }
}

public static class PauseTimerPositionExtensions
{
    internal static bool IsLeft(this PauseTimerPosition self) => self == PauseTimerPosition.BelowUI || self == PauseTimerPosition.CenterLeft || self == PauseTimerPosition.BottomLeft;

    internal static bool IsRight(this PauseTimerPosition self) => self == PauseTimerPosition.BottomRight || self == PauseTimerPosition.CenterRight || self == PauseTimerPosition.TopRight;

    internal static bool IsTop(this PauseTimerPosition self) => self == PauseTimerPosition.BelowUI || self == PauseTimerPosition.TopCenter || self == PauseTimerPosition.TopRight;

    internal static bool IsBottom(this PauseTimerPosition self) => self == PauseTimerPosition.BottomLeft || self == PauseTimerPosition.BottomCenter || self == PauseTimerPosition.BottomRight;

    internal static bool IsVCenter(this PauseTimerPosition self) => self == PauseTimerPosition.CenterLeft || self == PauseTimerPosition.CenterRight;

    public static PauseTimerSpacingParameters SpacingParameters(this PauseTimerPosition self)
    {
        Vector2 anchorPos;
        anchorPos.x = self.IsLeft() ? -14.5f : (self.IsRight() ? 14.5f : 0);
        anchorPos.y = self.IsTop() ? 7.5f : (self.IsBottom() ? -8.25f : 0);
        if (self == PauseTimerPosition.BelowUI) anchorPos.y = 4.5f;
        if (self == PauseTimerPosition.BottomLeft) anchorPos.y = -4.5f;

        Vector2 forwardSpace = new(0, self.IsTop() ? -1 : (self.IsVCenter() ? -0.5f : 0));
        Vector2 reverseSpace = new(0, self.IsBottom() ? 1 : (self.IsVCenter() ? 0.5f : 0));
        return new(self, anchorPos, forwardSpace, reverseSpace);
    }
}
