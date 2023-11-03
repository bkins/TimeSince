namespace TimeSince.Avails.Exceptions;

public class DatabaseCommitFailedException : Exception
{
    public DatabaseCommitFailedException(string message)
        : base(message)
    {

    }

    public DatabaseCommitFailedException(string message
                                       , Exception innerException)
        : base(message
             , innerException)
    {

    }
}
