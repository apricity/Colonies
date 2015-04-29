namespace Wacton.Colonies.Domain.Core
{
    public static class Logger
    {
        /* just an example of how to interact with and test this static class */
        private static bool loggerFlag; // defaults to "false" if not set

        public static void SetFlag(bool flag)
        {
            loggerFlag = flag;
        }

        public static bool GetFlag()
        {
            return loggerFlag;
        }
    }
}
