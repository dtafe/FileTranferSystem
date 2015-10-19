using FTSS.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTSS.Domain
{
    [Serializable]
    public class FileInfoMation
    {
        private string _File_Sharing_ID;
        private string _File_Name;
        private string _File_Size;
        private string _Upload_Date;

        public string File_Sharing_ID
        {
            get { return _File_Sharing_ID; }
            set { _File_Sharing_ID = value; }
        }
        public string File_Name
        {
            get { return _File_Name; }
            set { _File_Name = value; }
        }
        public string File_Size
        {
            get { return _File_Size; }
            set { _File_Size = value; }
        }

        public string Upload_Date
        {
            get { return _Upload_Date; }
            set { _Upload_Date = value; }
        }

        #region[File IDataReader]
        public FileInfoMation CustomerIDataReader(IDataReader dr)
        {
            FileInfoMation obj = new FileInfoMation();
            obj.File_Sharing_ID = (dr["File_Sharing_ID"] is DBNull) ? string.Empty : dr["File_Sharing_ID"].ToString();
            obj.File_Name = (dr["File_Name"] is DBNull) ? string.Empty : dr["File_Name"].ToString();
            obj.File_Size = (dr["File_Size"] is DBNull) ? string.Empty : dr["File_Size"].ToString();
            obj.Upload_Date = (dr["Upload_Date"] is DBNull) ? string.Empty : dr["Upload_Date"].ToString();
            return obj;
        }
        #endregion
    }
    #region FileControl
    public class FileControl
    {
        #region[File_GetByAll]
        public List<FileInfoMation> File_GetByAll()
        {
            List<FileInfoMation> list = new List<FileInfoMation>();
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand cmd = new SqlCommand("DDV_GetFileInfo", Conn))
                    {
                        FileInfoMation obj = new FileInfoMation();
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                list.Add(obj.CustomerIDataReader(dr));
                            }
                        }
                        dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Conn.Close();
                }
            }
            return list;
        }
        #endregion
        #region[File_Insert]
        public void File_Insert(FileInfoMation data)
        {
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand dbCmd = new SqlCommand("DDV_InsertFileInfo", Conn))
                    {
                        dbCmd.CommandType = CommandType.StoredProcedure;
                        dbCmd.Parameters.Add(new SqlParameter("@File_Sharing_ID", data.File_Sharing_ID));
                        dbCmd.Parameters.Add(new SqlParameter("@File_Name", data.File_Name));
                        dbCmd.Parameters.Add(new SqlParameter("@File_Size", data.File_Size));
                        dbCmd.Parameters.Add(new SqlParameter("@Upload_Date", data.Upload_Date));
                        dbCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Conn.Close();
                }
            }
        }
        #endregion
        #region[Customer_Update]
        public void File_Update(FileInfoMation data)
        {
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand dbCmd = new SqlCommand("sp_Customer_Update", Conn))
                    {
                        dbCmd.CommandType = CommandType.StoredProcedure;
                        dbCmd.Parameters.Add(new SqlParameter("@File_Sharing_ID", data.File_Sharing_ID));
                        dbCmd.Parameters.Add(new SqlParameter("@File_Name", data.File_Name));
                        dbCmd.Parameters.Add(new SqlParameter("@File_Size", data.File_Size));
                        dbCmd.Parameters.Add(new SqlParameter("@Upload_Date", data.Upload_Date));
                        dbCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion
        #region[File_Delete]
        public void File_Delete(string Id, string Name)
        {
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand dbCmd = new SqlCommand("DDV_DeleteFileInfo", Conn))
                    {
                        dbCmd.CommandType = CommandType.StoredProcedure;
                        dbCmd.Parameters.Add(new SqlParameter("@File_Sharing_ID", Id));
                        dbCmd.Parameters.Add(new SqlParameter("@File_Name", Name));
                        dbCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion
    }
    #endregion

    #region FileCallControl
    public class FileInforMa
    {
        private static FileControl db = new FileControl();

        //Customer get by all
        public static List<FileInfoMation> File_GetByAll()
        {
            return db.File_GetByAll();
        }

        //Customer Update
        public static void File_Update(FileInfoMation obj)
        {
            db.File_Update(obj);
        }

        //Customer Insert
        public static void File_Insert(FileInfoMation obj)
        {
            db.File_Insert(obj);
        }

        //Customer Delete
        public static void File_Delete(string Id, string Name)
        {
            db.File_Delete(Id, Name);
        }
    }
    #endregion
}
