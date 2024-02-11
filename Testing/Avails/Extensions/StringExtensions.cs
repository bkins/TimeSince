using TimeSince.Avails.Extensions;

namespace Testing.Avails.Extensions;

public class StringExtensions
{
    [Theory]
    [InlineData("ThisIsOneTest", 4)]
    [InlineData("ThisIsAnotherTest", 4)]
    [InlineData("ATest", 2)]
    [InlineData("AAATest", 2)] //Words in all caps (usually Acronyms) are treated as a word.
    [InlineData("Test", 1)]
    [InlineData("", 1)] //SplitCamelCase "" which technically is one word
    [InlineData(null, 0)]
    public void SplitCamelCase_ValidInput_ReturnsCorrectNumberOfWords(string test, int numberOfWords)
    {
        var result              = test.SplitCamelCase();
        var actualNumberOfWords = result?.Split(' ').Length ?? 0;

        Assert.Equal(numberOfWords, actualNumberOfWords);
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("  ", true)]
    [InlineData("\t", true)]
    [InlineData("\n\r", true)]
    [InlineData(null, true)]
    [InlineData("aString", false)]
    [InlineData("  aString", false)]
    [InlineData("\taString", false)]
    [InlineData(" \taString", false)]
    [InlineData("\n\raString", false)]
    [InlineData(" \n\raString", false)]
    public void IsNullEmptyOrWhitespace_ValidInput_ReturnsCorrectValueBasedOnIfInputIsNullEmptyOrWhitespace(string test
                                      , bool   expected)
    {
        var result = test.IsNullEmptyOrWhitespace();

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("  ", false)]
    [InlineData("\t", false)]
    [InlineData("\n\r", false)]
    [InlineData(null, false)]
    [InlineData("aString", true)]
    [InlineData("  aString", true)]
    [InlineData("\taString", true)]
    [InlineData(" \taString", true)]
    [InlineData("\n\raString", true)]
    [InlineData(" \n\raString", true)]
    public void HasValue_ValidInput_ReturnsCorrectValueBasedOnIfInputIsNullEmptyOrWhitespace(string test
                                                                                           , bool   expected)
    {
        var result = test.HasValue();

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("test", false, "Test")]
    [InlineData("ThisIsOneTest", false, "Thisisonetest")]
    [InlineData("ThisIsATest", false, "Thisisatest")]
    [InlineData("ThisIsAATest", false, "Thisisaatest")]
    [InlineData("ThisIsAAATest", false, "Thisisaaatest")]
    [InlineData("This Is A Test", false, "This Is A Test")]
    [InlineData("this is a test", false, "This Is A Test")]
    [InlineData("this is aaa test", false, "This Is Aaa Test")]
    //Below are how the 'force' parameter affects the results
    [InlineData("this is AAA test", false, "This Is AAA Test")]
    [InlineData("this is AAA test", true, "This Is Aaa Test")]
    [InlineData("THIS IS A TEST", false, "THIS IS A TEST")]
    [InlineData("THIS IS A TEST", true, "This Is A Test")]
    public void ToTitleCase_ValidInPut_ReturnsCorrectStringValueWithAllWordsCapitalized(string test
                                                                                      , bool   force
                                                                                      , string expected)
    {
        var result = test.ToTitleCase(force);

        Assert.Equal(expected
                   , result);
    }

    [Theory]
    [InlineData("", 0, 0, 0, 0)]                // Empty string should return TimeSpan zero
    [InlineData("15", 0, 15, 0, 0)]             // Whole number should return TimeSpan in minutes
    [InlineData("1:30", 0, 1, 30, 0)]           // HH:mm format
    [InlineData("2:45:30", 2, 45, 30, 0)]       // HH:mm:ss format
    public void ToTimeSpan_ValidInput_ReturnsCorrectTimeSpan(string test
                                                           , int expectedHours
                                                           , int expectedMinutes
                                                           , int expectedSeconds
                                                           , int expectedMilliseconds)
    {
        //Arrange & Act
        var result = test.ToTimeSpan();

        // Assert
        Assert.Equal(expectedHours, result.Hours);
        Assert.Equal(expectedMinutes, result.Minutes);
        Assert.Equal(expectedSeconds, result.Seconds);
        Assert.Equal(expectedMilliseconds, result.Milliseconds);
    }

    [Theory]
    [InlineData(0, 0, 0, "00:00")]      // TimeSpan zero should return "00:00"
    [InlineData(1, 30, 0, "01:30:00")]  // HH:mm format
    [InlineData(2, 45, 30, "02:45:30")] // HH:mm:ss format
    [InlineData(0, 120, 0, "02:00:00")] // Only minutes, no hours
    [InlineData(0, 0, 90, "01:30")]     // Only seconds, no hours or minutes
    public void ToShortForm_ValidInput_ReturnsCorrectShortForm(int hours
                                                             , int minutes
                                                             , int seconds
                                                             , string expectedShortForm)
    {
        // Arrange
        var timeSpan = new TimeSpan(hours, minutes, seconds);

        // Act
        var result = timeSpan.ToShortForm();

        // Assert
        Assert.Equal(expectedShortForm, result);
    }

    [Theory]
    [InlineData("yes", true)]     // "yes" should return true
    [InlineData("Y", true)]       // "Y" should return true
    [InlineData("true", true)]    // "true" should return true
    [InlineData("T", true)]       // "T" should return true
    [InlineData("no", false)]     // "no" should return false
    [InlineData("false", false)]  // "false" should return false
    [InlineData("random", false)] // Any other value should return false
    [InlineData("", false)]       // Empty string should return false
    [InlineData("0", false)]      // "0"" should return false
    [InlineData(null, false)]     // Null string should return false
    public void IsTrue_ValidInput_ReturnsCorrectResult(string test, bool expectedResult)
    {
        // Arrange & Act
        var result = test.IsTrue();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("123", 123)]   // Valid integer string should return the integer value
    [InlineData("-456", -456)] // Valid negative integer string should return the negative integer value
    [InlineData("456.5", 456)] // Valid negative integer string should return the negative integer value
    [InlineData("0", 0)]       // "0" should return 0
    [InlineData("invalid", 0)] // Invalid string should return 0
    [InlineData("", 0)]        // Empty string should return 0
    [InlineData(null, 0)]      // Null string should return 0
    public void ToSafeInt_ValidInput_ReturnsCorrectResult(string input, int expectedResult)
    {
        // Arrange & Act
        var result = input.ToSafeInt();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("123", 123.00)]         // Valid double string should return the double value
    [InlineData("123.45", 123.45)]         // Valid double string should return the double value
    [InlineData("-456.78", -456.78)]       // Valid negative double string should return the negative double value
    [InlineData("0.0", 0.0)]               // "0.0" should return 0.0
    [InlineData("invalid", 0.0)]           // Invalid string should return 0.0
    [InlineData("", 0.0)]                  // Empty string should return 0.0
    [InlineData(null, 0.0)]                // Null string should return 0.0
    public void ToSafeDouble_ValidInput_ReturnsCorrectResult(string input, double expectedResult)
    {
        // Arrange & Act
        var result = input.ToSafeDouble();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("hello world", "world", false, true)] // "hello world" contains "world"
    [InlineData("hello world", "World", true, true)] // Case-insensitive search
    [InlineData("hello world", "foo", false, false)]  // "hello world" does not contain "foo"
    [InlineData(null, "search", false, false)]        // Null value should return false
    [InlineData("", "", false, true)]                 // Empty value contains an empty search
    [InlineData("value", null, false, true)]          // Null search should return true
    [InlineData(null, null, false, true)]             // Null value and search should return true
    public void SafeContains_ValidInput_ReturnsCorrectResult(string value, string? searchValue, bool caseInsensitive, bool expectedResult)
    {
        // Arrange & Act
        var result = value.SafeContains(searchValue, caseInsensitive);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("hello world", "world", false, false)] // "hello world" contains "world", return false
    [InlineData("hello world", "World", true, false)]  // Case-insensitive search, return false
    [InlineData("hello world", "foo", false, true)]    // "hello world" does not contain "foo", return true
    [InlineData(null, "search", false, true)]         // Null value should return true
    [InlineData("", "", false, false)]                 // Empty value contains an empty search
    [InlineData("value", null, false, false)]          // Null search should return true
    [InlineData(null, null, false, false)]             // Null value and search should return true
    public void NotContains_WithCaseInsensitive_ValidInput_ReturnsCorrectResult(string  value
                                                                              , string? searchValue
                                                                              , bool    caseInsensitive
                                                                              , bool    expectedResult)
    {
        // Arrange & Act
        var result = value.NotContains(searchValue, caseInsensitive);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}
