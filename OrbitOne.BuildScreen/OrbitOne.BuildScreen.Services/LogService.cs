using System;
using log4net;

namespace OrbitOne.BuildScreen.Services
{
    public class LogService
    {
        private static ILog Logger = LogManager.GetLogger(typeof (LogService));

        public static void WriteInfo(string message)
        {
            Logger.Info(message);
        }

        public static void WriteError(Exception ex)
        {
            Logger.ErrorFormat("Exception {0} \n {1}", ex.Message, ex.StackTrace); 
        }
    }
}
