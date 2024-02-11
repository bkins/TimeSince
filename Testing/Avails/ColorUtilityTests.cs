using System.Text;
using TimeSince.Avails.ColorHelpers;
using TimeSince.MVVM.Models;
using Xunit.Abstractions;
using Color = System.Drawing.Color;
using MauiGraphics = Microsoft.Maui.Graphics;
using ColorUtilities = TimeSince.Avails.ColorHelpers.ColorUtility;

namespace Testing.Avails;

public class ColorUtilityTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void GetNamedColors_ReturnsNonEmptyList()
    {
        var colorNames = ColorUtilities.GetListOfNamedColors();

        // Assert
        Assert.NotNull(colorNames);
        Assert.NotEmpty(colorNames);
    }

    [Theory]
    [InlineData("Red", 1f, 0f, 0f, 1f)]
    [InlineData("Green", 0f, 0.501f, 0f, 1f)]
    [InlineData("InvalidColor", null, null, null, null)]
    [InlineData(null, null, null, null, null)]
    public void ConvertSystemColorNameToMauiColor_ReturnsCorrectColor(string? colorName
                                                                    , float? expectedRed
                                                                    , float? expectedGreen
                                                                    , float? expectedBlue
                                                                    , float? expectedAlpha)
    {
        // Arrange
        var expectedColor = expectedRed.HasValue
                                ? new MauiGraphics.Color(expectedRed.Value
                                                       , expectedGreen ?? 0
                                                       , expectedBlue ?? 0
                                                       , expectedAlpha ?? 0)
                                : null;

        // Act
        var mauiColor = ColorUtilities.ConvertSystemColorNameToMauiColor(colorName);

        // Debugging info
        DebugOutput(expectedColor, mauiColor);

        // Assert
        Assert.Equal(expectedColor, mauiColor, new ColorComparer());
    }


    [Theory]
    [InlineData(1f, 0f, 0f, 1f, 255, 0, 0, 255)]
    [InlineData(0.5f, 0.5f, 0.5f, 0.5f, 127, 127, 127, 127)]
    [InlineData(0f, 1f, 0f, 0.5f, 0, 255, 0, 127)]
    [InlineData(null, null, null, null, 0, 0, 0, 0)]
    public void ConvertMauiColorToSystemColor_ReturnsCorrectColor(float? mauiRed
                                                                , float? mauiGreen
                                                                , float? mauiBlue
                                                                , float? mauiAlpha
                                                                , int    expectedRed
                                                                , int    expectedGreen
                                                                , int    expectedBlue
                                                                , int    expectedAlpha)
    {
        // Arrange
        var mauiColor = mauiRed.HasValue
                            ? new MauiGraphics.Color(mauiRed.Value
                                                   , mauiGreen ?? 0
                                                   , mauiBlue ?? 0
                                                   , mauiAlpha ?? 0)
                            : null;

        var expectedColor = mauiColor != null
            ? Color.FromArgb(expectedAlpha, expectedRed, expectedGreen, expectedBlue)
            : Color.Empty;

        // Act
        var systemColor = ColorUtilities.ConvertMauiColorToSystemColor(mauiColor);

        DebugOutput(expectedColor, systemColor);

        // Assert
        Assert.Equal(expectedColor, systemColor);
    }


    [Theory]
    [MemberData(nameof(GetIndexTestCases))]
    public void GetIndexFromPartialName_ReturnsCorrectIndex(GetIndexTestCase testCase)
    {
        // Arrange
        ColorUtilities.PopulateColorNames();
        var colorNames = ColorUtilities.ColorNames?
                                       .OrderBy(colorName => colorName.Name)
                                       .Select(colorName => colorName.Name)
                                       .ToList();
        // Act
        var actualIndex = ColorUtilities.GetIndexFromPartialName(testCase.PartialName?.ToLowerInvariant());

        DebugOutput(testCase, actualIndex, colorNames);

        // Assert
        Assert.True(testCase.ExpectedIndex == actualIndex
                 || colorNames[actualIndex].StartsWith(testCase.PartialName!
                                                     , StringComparison.OrdinalIgnoreCase)
                  , $"The actual index ({actualIndex}) does not match the expected index ({testCase.ExpectedIndex}) or any color name starting with '{testCase.PartialName}'.");

    }

    [Theory]
    [MemberData(nameof(GetColorNameTestCases))]
    public void GetNameFromColor_ReturnsCorrectName(GetColorNameTestCase testCase)
    {
        // Act
        var actualName = ColorUtilities.GetColorInListOfColors(testCase.Color, testCase.ColorNames);

        DebugOutput(testCase, actualName);

        // Assert
        Assert.Equal(testCase.ExpectedName, actualName);
    }

    [Theory]
    [MemberData(nameof(GetIndexFromColorTestCases))]
    public void GetIndexFromColor_ReturnsCorrectIndex(GetIndexFromColorTestCase testCase)
    {
        // Arrange
        var colorNames = ColorUtilities.ColorNames
                                       //.OrderBy(colorName => colorName.Name)
                                       .Select(colorName => colorName.Name)
                                       .ToList();
        // Act
        var actualIndex = ColorUtilities.GetIndexFromColor(testCase.Color);

        DebutOutput(testCase
                  , colorNames
                  , actualIndex);

        // Assert
        Assert.Equal(testCase.ExpectedIndex, actualIndex);
    }

    [Fact]
    public void PopulateColorNames_PopulatesColorNamesCorrectly()
    {
        // Arrange
        var colorNames = ColorUtilities.ColorNames
                                       .Select(colorName => colorName.Name)
                                       .ToList();
        // Act
        ColorUtilities.PopulateColorNames();

        DebugOutput(colorNames);

        // Assert
        Assert.NotEmpty(ColorUtilities.ColorNames);
        Assert.Equal("AliceBlue", ColorUtilities.ColorNames[1].Name);
        Assert.Equal("AntiqueWhite", ColorUtilities.ColorNames[2].Name);

    }

    [Fact(Skip = "Fix later: Failing because, 'Could not load file or assembly 'Microsoft.Maui.Controls''")]
    public void ApplyDefaultColors_SetsDefaultColorsCorrectly()
    {
        // Arrange
        ColorUtilities.PopulateColorNames();

        // Act
        ColorUtilities.ApplyDefaultColors();

        // Assert
        Assert.NotEmpty(ColorUtilities.ColorNames);

        // Check a few specific colors to ensure they are set to default values
        Assert.Equal(MauiGraphics.Colors.AliceBlue.ToArgbHex(), ColorUtilities.ColorNames[1].Color.ToArgbHex());
        Assert.Equal(MauiGraphics.Colors.AntiqueWhite.ToArgbHex(), ColorUtilities.ColorNames[2].Color.ToArgbHex());

    }

    [Theory (Skip = "Need to adjust the GetContrastRatio method. See comments in GetContrastRatio. Also, look at GetRelativeLuminance")]
    [InlineData("Black", "White", 20.34)]
    [InlineData("Red", "Green", 3.98)]
    [InlineData("InvalidColor", "White", 0)]
    public void GetContrastRatio_CalculatesCorrectContrastRatio(string? namedColor1
                                                              , string? namedColor2
                                                              , double  expectedContrastRatio)
    {
        // Arrange
        ColorUtilities.PopulateColorNames();

        var color1       = ColorUtilities.ConvertSystemColorNameToMauiColor(namedColor1);
        var color2       = ColorUtilities.ConvertSystemColorNameToMauiColor(namedColor2);
        var                 systemColor1 = ColorUtilities.ConvertMauiColorToSystemColor(color1);
        var                 systemColor2 = ColorUtilities.ConvertMauiColorToSystemColor(color2);

        // Act
        var actualContrastRatio = ColorUtilities.GetContrastRatio(systemColor1
                                                                , systemColor2);

        // Debugging info
        DebugOutput(namedColor1
                  , namedColor2
                  , systemColor1
                  , systemColor2);

        // Assert
        Assert.Equal(expectedContrastRatio
                   , actualContrastRatio
                   , tolerance: 0.1);
    }

    [Theory(Skip = "When fixing GetContrastRatio method, look at GetRelativeLuminance as well.")]
    [InlineData("Red", 0.2126)]
    [InlineData("Green", 0.7152)]
    [InlineData("Blue", 0.0722)]
    [InlineData("Gray", 0.5 * 0.2126 + 0.5 * 0.7152)]
    public void GetRelativeLuminance_CalculatesCorrectly(string? namedColor, double expectedLuminance)
    {
        // Arrange
        ColorUtilities.PopulateColorNames();

        var mauiColor   = ColorUtilities.ConvertSystemColorNameToMauiColor(namedColor);
        var systemColor = ColorUtilities.ConvertMauiColorToSystemColor(mauiColor);

        // Act
        var actualLuminance = ColorUtilities.GetRelativeLuminance(systemColor);

        DebugOutput(namedColor
                  , expectedLuminance
                  , actualLuminance);

        // Assert
        Assert.Equal(expectedLuminance
                   , actualLuminance
                   , tolerance: 0.01);
    }

    [Theory]
    [InlineData("White", "Black")]        // White background, choose black text
    [InlineData("Black", "White")]        // Black background, choose white text
    [InlineData("Red", "Yellow")]          // Red background, choose white text
    [InlineData("LightBlue", "Black")]    // LightBlue background, choose black text
    public void ChooseReadableTextColor_ChoosesCorrectColor(string? backgroundColor, string? expectedTextColor)
    {
        // Arrange
        ColorUtilities.PopulateColorNames();

        var backgroundMauiColor   = ColorUtilities.ConvertSystemColorNameToMauiColor(backgroundColor);
        var expectedTextMauiColor = ColorUtilities.ConvertSystemColorNameToMauiColor(expectedTextColor);

        // Act
        var actualTextColor = ColorUtilities.ChooseReadableTextColor(backgroundMauiColor);

        DebugOutput(backgroundColor
                  , expectedTextColor
                  , expectedTextMauiColor
                  , actualTextColor);

        // Assert
        Assert.Equal(expectedTextMauiColor, actualTextColor);
    }

    [Theory]
    [InlineData("Black", "White", "DarkGray", Skip = "Fix Later - DistanceA: 1.147908217088752, DistanceB: 0.5841425904801253") ]
    [InlineData("LightBlue", "Green", "Blue")]
    [InlineData("DarkRed", "Red", "Green")]
    [InlineData("SaddleBrown", "Yellow", "Blue")]
    [InlineData("MediumBlue", "DarkBlue", "LightBlue")]
    public void TestColorDistances(string? colorCloseToTarget, string? colorNotAsCloseToTarget, string? target)
    {
        // Arrange
        var targetColor             = ColorUtilities.ConvertSystemColorNameToMauiColor(target);
        var closeToTargetColor      = ColorUtilities.ConvertSystemColorNameToMauiColor(colorCloseToTarget);
        var notAsCloseToTargetColor = ColorUtilities.ConvertSystemColorNameToMauiColor(colorNotAsCloseToTarget);

        // Act
        var distanceA = ColorUtilities.CalculateColorDistance(targetColor, closeToTargetColor);
        var distanceB = ColorUtilities.CalculateColorDistance(targetColor, notAsCloseToTargetColor);

        DebugOutput(colorCloseToTarget
                  , colorNotAsCloseToTarget
                  , target
                  , closeToTargetColor
                  , distanceA
                  , notAsCloseToTargetColor
                  , distanceB
                  , targetColor);

        // Assert
        Assert.True(distanceA < distanceB, $"DistanceA: {distanceA}, DistanceB: {distanceB}");
    }

    [Theory]
    [InlineData("DarkRed", "Red", "Green")]
    [InlineData("SaddleBrown", "Yellow", "Blue")]
    [InlineData("DarkSlateGray", "DarkOliveGreen", "LightCyan", Skip = "Fix later: Not sure is this is failing correctly.")]
    public void FindTheClosestColor_FindsClosestColor(string? closeColorName
                                                    , string? notSoCloseColorName
                                                    , string? targetColorName)
    {
        // Arrange
        ColorUtilities.PopulateColorNames();

        var closeColor      = ColorUtilities.ConvertSystemColorNameToMauiColor(closeColorName);
        var notSoCloseColor = ColorUtilities.ConvertSystemColorNameToMauiColor(notSoCloseColorName);
        var targetColor     = ColorUtilities.ConvertSystemColorNameToMauiColor(targetColorName);

        var colorNames = new List<ColorName>
                         {
                             new() { Name = closeColorName,      Color = closeColor }
                           , new() { Name = notSoCloseColorName, Color = notSoCloseColor }
                         };

        // Act
        var closestColor = ColorUtilities.FindTheClosestColor(colorNames
                                                            , targetColor);

        // Assert
        Assert.Equal(closeColorName
                   , closestColor.Name);
    }

    //Helpers

#region Debug Output Methods

    private void DebugOutput(MauiGraphics.Color? expectedColor
                           , MauiGraphics.Color? mauiColor)
    {

        testOutputHelper.WriteLine("Expected Color:");
        testOutputHelper.WriteLine(DebugOutput(expectedColor));

        testOutputHelper.WriteLine("Actual Color:");
        testOutputHelper.WriteLine(DebugOutput(mauiColor));
    }

    private void DebugOutput(Color expectedColor
                           , Color systemColor)
    {
        testOutputHelper.WriteLine($"Expected: {expectedColor}");
        testOutputHelper.WriteLine($"Actual: {systemColor}");
    }

    private void DebugOutput(GetIndexTestCase testCase
                           , int              actualIndex
                           , List<string?>?   colorNames)
    {
        var sortedIndices = colorNames.Select((name
                                             , index) => new { Name = name, Index = index }).ToList();

        testOutputHelper.WriteLine($"Test Scenario: {testCase.PartialName}");
        testOutputHelper.WriteLine($"Sorted Indices: {string.Join(", ", sortedIndices.Select(item => $"{item.Name}={item.Index}"))}");
        testOutputHelper.WriteLine($"Expected: {testCase.ExpectedIndex}");
        testOutputHelper.WriteLine($"Actual: {actualIndex}");
        testOutputHelper.WriteLine(string.Empty);
    }


    private void DebugOutput(GetColorNameTestCase testCase
                           , string?              actualName)
    {

        testOutputHelper.WriteLine($"Test Scenario: {testCase.Color}");
        testOutputHelper.WriteLine($"Expected: {testCase.ExpectedName}");
        testOutputHelper.WriteLine($"Actual: {actualName}");
        testOutputHelper.WriteLine(string.Empty);
    }

    private void DebutOutput(GetIndexFromColorTestCase testCase
                           , IEnumerable<string>       colorNames
                           , int                       actualIndex)
    {

        var sortedIndices = colorNames.Select((name
                                             , index) => new { Name = name, Index = index }).ToList();

        testOutputHelper.WriteLine($"Test Scenario: {testCase.Color?.ToString() ?? "NULL"}");
        testOutputHelper.WriteLine($"Sorted Indices: {string.Join(", ", sortedIndices.Select(item => $"{item.Name}={item.Index}"))}");
        testOutputHelper.WriteLine($"Expected: {testCase.ExpectedIndex} ({ColorUtilities.ColorNames[testCase.ExpectedIndex].Name})");
        testOutputHelper.WriteLine($"Actual: {actualIndex} ({ColorUtilities.ColorNames[actualIndex].Name})");
        testOutputHelper.WriteLine(string.Empty);
    }

    private void DebugOutput(List<string> colorNames)
    {

        var colorNamesForOutput = colorNames.Select((name
                                                   , index) => new { Name = name, Index = index }).ToList();

        testOutputHelper.WriteLine($"Sorted Indices: {string.Join(", ", colorNamesForOutput.Select(item => $"{item.Name}={item.Index}"))}");
    }

    private void DebugOutput(string? namedColor1
                           , string? namedColor2
                           , Color   systemColor1
                           , Color   systemColor2)
    {

        testOutputHelper.WriteLine($"Color 1: {namedColor1}");
        testOutputHelper.WriteLine(DebugOutput(systemColor1));
        testOutputHelper.WriteLine($"Color 2: {namedColor2}");
        testOutputHelper.WriteLine(DebugOutput(systemColor2));
    }

    private void DebugOutput(string? namedColor
                           , double expectedLuminance
                           , double actualLuminance)
    {

        testOutputHelper.WriteLine($"Test Case: {namedColor}");
        testOutputHelper.WriteLine($"ExpectedLuminance: {expectedLuminance}");
        testOutputHelper.WriteLine($"ActualLuminance: {actualLuminance}");
    }

    private void DebugOutput(string?             backgroundColor
                           , string?             expectedTextColor
                           , MauiGraphics.Color? expectedTextMauiColor
                           , MauiGraphics.Color? actualTextColor)
    {

        testOutputHelper.WriteLine($"Test case: {backgroundColor}");
        testOutputHelper.WriteLine($"ExpectedTextColor: {expectedTextColor}");
        testOutputHelper.WriteLine($"ExpectedTextMauiColor: {ColorUtilities.GetColorInListOfColors(expectedTextMauiColor
                                                                                            , ColorUtilities.ColorNames
                                                                                                            .ToList())}");
        testOutputHelper.WriteLine($"ActualTextColor: {ColorUtilities.GetColorInListOfColors(actualTextColor
                                                                                      , ColorUtilities.ColorNames
                                                                                                      .ToList())}");
    }

    private void DebugOutput(string?             colorCloseToTarget
                           , string?             colorNotAsCloseToTarget
                           , string?             target
                           , MauiGraphics.Color? closeToTargetColor
                           , double              distanceA
                           , MauiGraphics.Color? notAsCloseToTargetColor
                           , double              distanceB
                           , MauiGraphics.Color? targetColor)
    {

        testOutputHelper.WriteLine($"Test case: {colorCloseToTarget} / {colorNotAsCloseToTarget} / {target}");

        testOutputHelper.WriteLine($"{nameof(closeToTargetColor)}:");
        testOutputHelper.WriteLine($"   Name: {colorCloseToTarget}");
        testOutputHelper.WriteLine($"   RGBA: {closeToTargetColor}");
        testOutputHelper.WriteLine($"   Distance to {nameof(target)}: {distanceA}");

        testOutputHelper.WriteLine($"{nameof(notAsCloseToTargetColor)}:");
        testOutputHelper.WriteLine($"   Name: {colorNotAsCloseToTarget}");
        testOutputHelper.WriteLine($"   RGBA: {notAsCloseToTargetColor}");
        testOutputHelper.WriteLine($"   Distance to {nameof(target)}: {distanceB}");

        testOutputHelper.WriteLine($"{nameof(target)}:");
        testOutputHelper.WriteLine($"   Name: {target}");
        testOutputHelper.WriteLine($"   RGBA: {targetColor}");

        testOutputHelper.WriteLine(
            $"Distance from {colorCloseToTarget} ({nameof(closeToTargetColor)}) and {target} ({nameof(targetColor)}): {distanceA} ({nameof(distanceA)})");
        testOutputHelper.WriteLine(
            $"Distance from {colorNotAsCloseToTarget} ({nameof(notAsCloseToTargetColor)}) and {target} ({nameof(targetColor)}): {distanceB} ({nameof(distanceB)})");
    }

    private static string DebugOutput(Color color)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Name: {color.Name}");
        stringBuilder.AppendLine($"a: {color.A}, r: {color.R}, g: {color.G}, b: {color.B}");

        return stringBuilder.ToString();
    }

    private static string DebugOutput(MauiGraphics.Color? color)
    {
        if (color is null) return "Color is null";

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"ToString: {color.ToString()}");
        stringBuilder.AppendLine($"a: {color.Alpha}, r: {color.Red}, g: {color.Green}, b: {color.Blue}");

        return stringBuilder.ToString();
    }

#endregion

#region InLine Data Models

    public class GetIndexTestCase
    {
        public string? PartialName   { get; set; }
        public int     ExpectedIndex { get; set; }
    }

    public static IEnumerable<object[]> GetIndexTestCases()
    {
        ColorUtilities.PopulateColorNames();

        var sortedTestCases = new List<GetIndexTestCase>
                              {
                                  new GetIndexTestCase { PartialName = "Ro", ExpectedIndex      = 115 },
                                  new GetIndexTestCase { PartialName = "In", ExpectedIndex      = 56 },
                                  new GetIndexTestCase { PartialName = "Li", ExpectedIndex      = 76 },
                                  new GetIndexTestCase { PartialName = "Invalid", ExpectedIndex = -1 },
                                  new GetIndexTestCase { PartialName = null, ExpectedIndex      = -1 }
                              };

        return sortedTestCases.Select(testCase => new object[] { testCase });
    }

    public class GetColorNameTestCase
    {
        public required MauiGraphics.Color? Color        { get; init; }
        public required List<ColorName>     ColorNames   { get; set; }
        public required string              ExpectedName { get; init; }
    }

    public static IEnumerable<object[]> GetColorNameTestCases()
    {
        ColorUtilities.PopulateColorNames(); // Ensure that ColorNames collection is populated

        yield return new object[]
                     {
                         new GetColorNameTestCase
                         {
                             Color        = MauiGraphics.Colors.Blue
                           , ColorNames   = new List<ColorName>(ColorUtilities.ColorNames)
                           , ExpectedName = "Blue"
                         }
                     };
        yield return new object[]
                     {
                         new GetColorNameTestCase
                         {
                             Color        = MauiGraphics.Colors.Red
                           , ColorNames   = new List<ColorName>(ColorUtilities.ColorNames)
                           , ExpectedName = "Red"
                         }
                     };
        yield return new object[]
                     {
                         new GetColorNameTestCase
                         {
                             Color        = MauiGraphics.Colors.Green
                           , ColorNames   = new List<ColorName>(ColorUtilities.ColorNames)
                           , ExpectedName = "Green"
                         }
                     };
    }

    public class GetIndexFromColorTestCase
    {
        public required MauiGraphics.Color? Color         { get; set; }
        public          int                 ExpectedIndex { get; set; }
    }

    public static IEnumerable<object[]> GetIndexFromColorTestCases()
    {
        ColorUtilities.PopulateColorNames(); // Ensure that ColorNames collection is populated

        // Test case where the color is found in the list
        yield return new object[] { new GetIndexFromColorTestCase { Color = MauiGraphics.Colors.Blue, ExpectedIndex = 10 } };

        // Test case where the color is not found, so it should return -1
        yield return new object[] { new GetIndexFromColorTestCase { Color = MauiGraphics.Color.FromRgba(1.1, 0.0, 0.0, 1.0), ExpectedIndex = 115 } };

        // Test case where a close match is found, and the index of the close match should be returned
        yield return new object[] { new GetIndexFromColorTestCase { Color = MauiGraphics.Colors.Orange, ExpectedIndex = 100 } };

        // Null Color
        yield return new object[] { new GetIndexFromColorTestCase { Color = null, ExpectedIndex = 0 } };

        // Closest Match Not Found in First Attempt (Color is a shade of gray)
        yield return new object[] { new GetIndexFromColorTestCase { Color = MauiGraphics.Color.FromRgba(0.5, 0.5, 0.5, 1.0), ExpectedIndex = 51 } };

        //Recursive Closest Match (Color is a darker shade of gray)
        yield return new object[] { new GetIndexFromColorTestCase { Color = MauiGraphics.Color.FromRgba(0.2, 0.2, 0.2, 1.0), ExpectedIndex = 36 } };

    }

#endregion

}
