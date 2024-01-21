namespace TimeSince.Avails.ColorHelpers;

public class ColorComparer : IEqualityComparer<Color?>
{
    private const float Epsilon = 0.001f;

    public bool Equals(Color? x
                     , Color? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;

        return Math.Abs(x.Alpha - y.Alpha) < Epsilon
            && Math.Abs(x.Red - y.Red) < Epsilon
            && Math.Abs(x.Green - y.Green) < Epsilon
            && Math.Abs(x.Blue - y.Blue) < Epsilon;
    }

    public int GetHashCode(Color? obj)
    {
        return obj?.GetHashCode() ?? 0;
    }
}
