using FTSS.Domain;
using FTSS.Utilities;
using FTSS.Web.Form;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace FTSS.Web.WebService
{
    /// <summary>
    /// Summary description for WSDeleteFiles
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WSDeleteFiles : System.Web.Services.WebService
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [WebMethod]
        public void DeleteFiles()
        {
            try
            {
                logger.Info("Begin WS DeleteFiles from Webservice");
                List<SharingInfo> listSharingInfo = new List<SharingInfo>();
                SharingInfo objSharingInfo = new SharingInfo();
                using (SqlConnection cnn = new SqlConnection(Common.GetConnectString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "select * from TD_SHARING_INFO where Expiration_date < getdate() and delete_flag = 0";
                    cmd.Connection = cnn;
                    cnn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            listSharingInfo.Add(objSharingInfo.CustomerIDataReader(dr));
                        }
                    }
                    cnn.Close();
                }

                if (listSharingInfo.Count > 0)
                {
                    if (Common.AppSettingKey(Constant.STORAGE_CONNECT_STRING) != "")
                    {
                        BlobManager blobManger = new BlobManager();
                        for (int i = 0; i < listSharingInfo.Count; i++)
                        {
                            blobManger.DeleteBlobDirectory(listSharingInfo[i].customer_id);
                            UpdateSharingInfo(listSharingInfo[i].ID);
                        }
                    }
                    else
                    {
                        string hostPath = GetMapPath();
                        for (int i = 0; i < listSharingInfo.Count; i++)
                        {
                            string pathCustomerID = System.IO.Path.Combine(hostPath, Constant.UPLOAD_STORAGE + "\\" + listSharingInfo[i].customer_id);
                            if (Directory.Exists(pathCustomerID))
                                Directory.Delete(pathCustomerID, true);

                            UpdateSharingInfo(listSharingInfo[i].ID);

                        }
                    }
                }

                logger.Info("End DeleteFiles from Webservice");
            }
            catch (Exception ex)
            {
                logger.Error("Error in DeleteFiles " , ex);
                throw ex;
            }

        }
        [WebMethod]
        public void DeleteDirectoryNotExistInDB()
        {
            try
            {
                if (Common.AppSettingKey(Constant.STORAGE_CONNECT_STRING) != "") //only case for has blob , manage blob
                {

                    logger.Debug("Begin DeleteDirectoryNotExistInDB ");
                    BlobManager blob = new BlobManager();
                    List<string> listDirectory = blob.GetDirectoryListInContainer();
                    string listCustomerId = "";
                    foreach (string i in listDirectory)
                    {
                        listCustomerId += string.Format("'{0}',", i);
                    }
                    if (listCustomerId.EndsWith(","))
                        listCustomerId = listCustomerId.Remove(listCustomerId.Length - 1, 1);


                    List<SharingInfo> listSharingInfo = new List<SharingInfo>();
                    using (SqlConnection cnn = new SqlConnection(Common.GetConnectString()))
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = System.Data.CommandType.Text;


                        cmd.CommandText = string.Format("select distinct S.customer_id from TD_SHARING_INFO as S , TD_FILE_INFO as F where S.ID = F.File_Sharing_ID and S.customer_id in ({0})", listCustomerId);
                        cmd.Connection = cnn;
                        cnn.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                SharingInfo objSharingInfo = new SharingInfo();
                                objSharingInfo.customer_id = (dr["customer_id"] is DBNull) ? string.Empty : dr["customer_id"].ToString();

                                listSharingInfo.Add(objSharingInfo);
                            }
                        }
                        cnn.Close();
                    }

                    if (listDirectory.Count > listSharingInfo.Count)
                    {
                        foreach (string i in listDirectory)
                        {
                            SharingInfo objShare = listSharingInfo.FirstOrDefault(p => p.customer_id == i);
                            if (objShare == null)
                            {
                                logger.Debug("Delete directory = " + i);
                                blob.DeleteBlobDirectory(i);
                            }
                        }
                    }
                    logger.Debug("End DeleteDirectoryNotExistInDB ");

                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in DeleteDirectoryNotExistInDB ", ex);
            }
        }

        private void UpdateSharingInfo(int ID)
        {
            SqlConnection cnn = new SqlConnection(Common.GetConnectString());
            SqlTransaction Trans = null;
            try
            {
               
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "update TD_SHARING_INFO set delete_flag = 1 where ID = '"+ID+"'";
                cmd.Connection = cnn;
                cnn.Open();
                Trans = cnn.BeginTransaction();
                cmd.Transaction = Trans;
                cmd.ExecuteNonQuery();
                Trans.Commit();
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                cnn.Close();
            }
           
        }
        private string GetMapPath()
        {
            return Server.MapPath("~/");
        }
    }
}
