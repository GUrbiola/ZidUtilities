namespace CommonCode.Log4Net.Read
{
    /// <summary>
    /// Enum with the different log4net message levels, such as DEBUG, INFO, WARN, ERROR, FATAL. 
    /// ALL is also included for filtering purposes, no message will have this value.
    /// </summary>
    public enum MessageLevel
    {
        ALL,
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL
    }
}
