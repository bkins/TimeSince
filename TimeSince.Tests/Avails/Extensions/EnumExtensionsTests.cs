using System.ComponentModel;
using TimeSince.Avails.Extensions;

namespace TimeSince.Tests.Avails.Extensions;

public class EnumExtensionsTests
{

    [Fact]
    public void GetDescription_ReturnsEnumStringWhenAttributeIsNull()
    {
        // Act
        var result = EnumValueWithoutDescription.GetDescription();

        // Assert
        Assert.Equal(EnumValueWithoutDescription.ToString(), result);
    }

    [Fact]
    public void GetDescription_ReturnsAttributeDescriptionWhenAttributeIsNotNull()
    {
        // Act
        var result = EnumValueWithDescription.GetDescription();

        // Assert
        Assert.Equal("Description for ValueWithDescription", result);
    }
#region Helpers

    private const TestEnum EnumValueWithDescription    = TestEnum.ValueWithDescription;
    private const TestEnum EnumValueWithoutDescription = TestEnum.ValueWithoutDescription;
    private enum TestEnum
    {
        [Description("Description for ValueWithDescription")]
        ValueWithDescription
      , ValueWithoutDescription
    }

#endregion

}
