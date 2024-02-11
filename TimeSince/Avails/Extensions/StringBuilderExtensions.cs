using System.Text;

namespace TimeSince.Avails.Extensions;

public static partial class StringBuilderExtensions
{
    public static void ConditionalAppendLine(this StringBuilder value
                                           , string             textToAppend
                                           , bool               conditional)
    {
        if (conditional)
        {
            value.AppendLine(textToAppend);
        }
    }
}
