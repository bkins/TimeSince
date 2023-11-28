using System.Drawing;
using Color = System.Drawing.Color;

namespace TimeSince.Avails.ColorHelpers;

public static class ColorInfo
{
    public static readonly string White  = nameof(White).ToLower();
    public static readonly string Gray   = nameof(Gray).ToLower();
    public static readonly string Red    = nameof(Red).ToLower();
    public static readonly string Yellow = nameof(Yellow).ToLower();
    public static readonly string Green  = nameof(Green).ToLower();
    public static readonly string Black  = nameof(Black).ToLower();

    public const string RoyalAzureAsHex     = "#512BD4";
    public const string LavenderMistAsHex   = "#DFD8F7";
    public const string MidnightIndigoAsHex = "#2B0B98";
    public const string BlackAsHex          = "#000000";
    public const string WhiteAsHex          = "#FFFFFF";
    public const string YellowAsHex         = "#FFFF00";

    public static Color RoyalAzure     = ColorTranslator.FromHtml(RoyalAzureAsHex);
    public static Color LavenderMist   = ColorTranslator.FromHtml(LavenderMistAsHex);
    public static Color MidnightIndigo = ColorTranslator.FromHtml(MidnightIndigoAsHex);

}
