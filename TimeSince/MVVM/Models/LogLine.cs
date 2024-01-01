using System.Text;
using TimeSince.Avails;
using TimeSince.Avails.ColorHelpers;
using TimeSince.Avails.Extensions;

namespace TimeSince.MVVM.Models
{
    public class LogLine
    {
        public string TimeStampAsHtml
        {
            get => $"<p style=\"color:gray\">{TimeStamp}</p>";
        }

        public string TimeStamp { get; set; }

        public string CategoryAsHtml
        {
            get
            {
                var category = Category.ToString();

                category = Category switch
                {
                    Category.Error => $"<p style=\"color:{ColorInfo.Red}\">{Category.ToString()}</p>"
                  , Category.Warning => $"<p style=\"color:{ColorInfo.Yellow}\">{Category.ToString()}</p>"
                  , Category.Information => $"<p style=\"color:{ColorInfo.Green}\">{Category.ToString()}</p>"
                  , _ => category
                };

                return category;
            }
        }

        public Category Category { get; set; }

        public string MessageAsHtml
        {
            get => $"<p style=\"color:{ColorInfo.Gray}\">{Message}</p>";
        }

        public string Message { get; set; }

        public DateTime TimestampDateTime
        {
            get => DateTime.Parse(TimeStamp);
        }

        public string ExtraDetails { get; set; }

        public LogLine ()
        {
            TimeStamp = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}";
        }
        
        public bool HasExtraDetails { get => ExtraDetails.HasValue(); }
        public bool IsVisible       { get; set; }

        public string ToString(bool formatAsHtml = false)
        {
            var extraLine = string.Empty;

            if (ExtraDetails.HasValue())
            {
                extraLine = $"{ExtraDetails}";
            }

            return formatAsHtml 
                ? BuildLineAsHtml() 
                : $"{TimeStamp} | {Category.ToString()} | {Message}{extraLine}";
        }

        private string BuildLineAsHtml()
        {
            var timeStamp = StyleTextWithColor(TimeStamp, ColorInfo.Gray);

            var categoryColor = Category switch
            {
                Category.Error => ColorInfo.Red
              , Category.Warning => ColorInfo.Yellow
              , Category.Information => ColorInfo.Green
              , _ => ColorInfo.Black
            };

            var category  = StyleTextWithColor(Category.ToString(), categoryColor);
            var message   = StyleTextWithColor(Message, ColorInfo.White);
            var extraLine = string.Empty;

            if ( ! ExtraDetails.HasValue())
                return $"{category}{timeStamp}{message}{extraLine}<hr style='margin-top:1.5em' />";

            extraLine = $"{ExtraDetails.Replace(@"\", @"_")}";
            extraLine = StyleTextWithColor(extraLine, ColorInfo.White);

            return $"{category}{timeStamp}{message}{extraLine}<hr style='margin-top:1.5em' />";
        }

        private string StyleTextWithColor(string text
                                        , string color)
        {
            var htmlLines = new StringBuilder();
            var lines = text.Split(Environment.NewLine.ToCharArray()
                                 , StringSplitOptions.None);
            
            foreach (var line in lines)
            {
                htmlLines.Append($"<p style=\"color:{color}\">{line}</p>");
            }

            return htmlLines.ToString();
        }

        public class LogLineComparer : IEqualityComparer<LogLine>
        {
            public bool Equals(LogLine x, LogLine y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;

                return x.Category == y.Category
                    && x.Message == y.Message
                    && x.ExtraDetails == y.ExtraDetails
                    && x.TimeStamp == y.TimeStamp;
            }

            public int GetHashCode(LogLine obj)
            {
                unchecked
                {
                    int hashCode = (int)obj.Category;
                    hashCode = (hashCode * 397) ^ (obj.Message != null ? obj.Message.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.ExtraDetails != null ? obj.ExtraDetails.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.TimeStamp != null ? obj.TimeStamp.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }
    }
}
