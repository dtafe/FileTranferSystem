using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTSS.AutoDeleteFiles.Properties;

namespace FTSS.AutoDeleteFiles
{
    class Program
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                logger.InfoFormat("Begin Run FTSS.AutoDeleteFile at time {0}" , DateTime.Now.ToShortDateString());

                DeleteFilesWS.WSDeleteFiles deleteFileWS = new DeleteFilesWS.WSDeleteFiles();
                deleteFileWS.Timeout = 1000000;
                deleteFileWS.Url = Settings.Default.FTSS_AutoDeleteFiles_DeleteFilesWS_WSDeleteFiles;

                if (Settings.Default.UserName == "")
                {
                    deleteFileWS.UseDefaultCredentials = true;
                }
                else
                {
                    deleteFileWS.Credentials = new System.Net.NetworkCredential(Settings.Default.UserName, Settings.Default.Password);
                }
                logger.Debug("call DeleteFiles");
                deleteFileWS.DeleteFiles();
                logger.Debug("Call DeleteDirectoryNotExistInDB");
                deleteFileWS.DeleteDirectoryNotExistInDB();

                logger.Debug("End Run FTSS.AutoDeleteFile ");
                logger.Debug("------------------------------------------------ ");
            }
            catch (Exception ex)
            {
                logger.Error("Error FTSS.AutoDeleteFile ", ex);
            } 
        }
    }
}
