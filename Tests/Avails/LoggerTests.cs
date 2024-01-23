using Newtonsoft.Json;
using TimeSince.Avails;
using TimeSince.MVVM.Models;
using Xunit.Abstractions;

namespace Tests.Avails
{
    public class LoggerTests(ITestOutputHelper testOutputHelper)
    {
        [Fact]
        public void LogError_AddsToLogListAndLogsToFile()
        {
            // Arrange
            const string expectedMessage          = "Test Error Message";
            const string expectedExceptionDetails = "Test Exception Details";
            const string expectedExtraDetails     = "Test Extra Details";

            var logger = new Logger(forProd: false) { ShouldLogToFile = true };
            logger.Clear(true);

            // Act
            logger.LogError(expectedMessage, expectedExceptionDetails, expectedExtraDetails);

            // Read existing file contents after the log has been written
            var fileContentsAfterLog = File.Exists(logger.FullLogPath)
                                                ? File.ReadAllText(logger.FullLogPath)
                                                : string.Empty;

            // Deserialize the file contents to a list of LogLine
            var loggedListAfterLog = JsonConvert.DeserializeObject<List<LogLine>>(fileContentsAfterLog) ?? [];

            // Assert
            var expectedLogList = new List<LogLine>
                                  {
                                      new()
                                      {
                                          Category     = Category.Error,
                                          Message      = expectedMessage,
                                          ExtraDetails = expectedExceptionDetails + expectedExtraDetails
                                      }
                                  };

            // Print out the contents for debugging
            testOutputHelper.WriteLine($"expectedLogList.Count: {expectedLogList.Count}");
            testOutputHelper.WriteLine($"loggedListAfterLog.Count: {loggedListAfterLog.Count}");
            testOutputHelper.WriteLine($"expectedLogList[0].Message: {expectedLogList[0].Message}");
            testOutputHelper.WriteLine($"loggedListAfterLog[0].Message: {loggedListAfterLog[0].Message}");

            // Assert that the logged list matches the expected list using LogLineComparer
            Assert.Equal(expectedLogList
                       , loggedListAfterLog
                       , new LogLine.LogLineComparer());
        }


        [Fact]
        public void LogTrace_AddsToLogListAndLogsToFile()
        {
            // Arrange
            const string expectedMessage = "Test Trace Message";

            var logger = new Logger(forProd: false);
            logger.Clear(softClearLogFile: true);

            // Act
            logger.LogTrace(expectedMessage);

            // Assert
            Assert.Single(Logger.LogList);
            Assert.Equal(Category.Information, Logger.LogList[0].Category);
            Assert.Equal(expectedMessage, Logger.LogList[0].Message);

            // Check if log is written to file
            var fileContents = File.ReadAllText(logger.FullLogPath);
            var loggedList = JsonConvert.DeserializeObject<List<LogLine>>(fileContents);

            Assert.NotNull(loggedList);
            Assert.Single(loggedList);
            Assert.Equal(Category.Information, loggedList[0].Category);
            Assert.Equal(expectedMessage, loggedList[0].Message);
        }
    }
}
