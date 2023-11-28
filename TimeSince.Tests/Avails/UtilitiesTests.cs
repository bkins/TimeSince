using System.ComponentModel;
using TimeSince.Avails;

namespace TimeSince.Tests.Avails;

public class UtilitiesTests
{
    [Fact]
    public void GetEnumValueFromDescription_Returns_CorrectValue()
    {
        // Arrange
        const string description = "Second";

        // Act
        var result = Utilities.GetEnumValueFromDescription<TestEnum>(description);

        // Assert
        Assert.Equal(TestEnum.SecondValue, result);
    }

    [Fact]
    public void GetEnumValueFromDescription_NoDescription_Returns_CorrectValue()
    {
        // Arrange
        const string description = "ThirdValue";

        // Act
        var result = Utilities.GetEnumValueFromDescription<TestEnum>(description);

        // Assert
        Assert.Equal(TestEnum.ThirdValue, result);
    }

    [Fact]
    public void GetEnumValueFromDescription_NotFound_ThrowsArgumentException()
    {
        // Arrange
        const string description = "NonExistentValue";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Utilities.GetEnumValueFromDescription<TestEnum>(description));
    }
}

public enum TestEnum
{
    [Description("First")]
    FirstValue,
    [Description("Second")]
    SecondValue,
    ThirdValue
}
