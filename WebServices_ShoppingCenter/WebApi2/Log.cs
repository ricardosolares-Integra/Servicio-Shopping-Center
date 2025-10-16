using NLog;
using System;

namespace ShoppingCenter.WebServices.Base
{
    public static class Log
    {
        public static Logger logger
        {
            get { return LogManager.GetCurrentClassLogger(); }
        }

        public static string msg(string error, string exception)
        {
            return error + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "  EXCEPTION:" + exception;
        }

        public static string msg(string error, Exception ex)
        {
            return error + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "  EXCEPTION:" + ex.Message + " INNER:" + ex.InnerException.Message;
        }
    }
}
