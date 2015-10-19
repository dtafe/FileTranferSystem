using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Globalization;
using log4net;

namespace FTSS.Utilities
{
    public static class Common
    {
        readonly static ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region AppSetting
        public static string AppSettingKey(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetConnectString()
        {
            try
            {
                return ConfigurationManager.ConnectionStrings[Constant.FTSS_CONNECT_STRING].ConnectionString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 

        #region Resource
        public static string GetResourceString(string key)
        {
            try
            {
                return ResourceUtil.Instance.GetString(key);
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        #endregion 

        #region Log4net
        public static log4net.ILog InitLog4Net()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                logger.Debug("Init log4net completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return logger;
        }
        #endregion 

        #region Size 
        public static string ConvertBytesToDisplayText(long byteCount)
        {
            string result = "";

            if (byteCount > Math.Pow(1024, 3))
            {
                // display as gb
                result = (byteCount / Math.Pow(1024, 3)).ToString("#,#.##", CultureInfo.InvariantCulture) + " GB";
            }
            else if (byteCount > Math.Pow(1024, 2))
            {
                // display as mb
                result = (byteCount / Math.Pow(1024, 2)).ToString("#,#.##", CultureInfo.InvariantCulture) + " MB";
            }
            else if (byteCount > 1024)
            {
                // display as kb
                result = (byteCount / 1024).ToString("#,#.##", CultureInfo.InvariantCulture) + " KB";
            }
            else
            {
                // display as bytes
                if (byteCount == 0)
                    result = "0 Bytes";
                else
                    result = byteCount.ToString("#,#.##", CultureInfo.InvariantCulture) + " Bytes";
            }
            return result;
        }
        #endregion 
        #region Get row string
        public static string GetRowString(string textString)
        {            
            return textString.Replace("&nbsp;","");
        }
        #endregion
        #region Contains Upper & Lower
        public static void Contains()
        {

        }
        #endregion
    }
}
