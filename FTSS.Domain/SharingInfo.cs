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
    #region SharingInfo
    public class SharingInfo
    {
        public SharingInfo()
        {
        }
        private int _ID { get; set; }
        private string _dd_member_account { get; set; }
        private string _Mode_code { get; set; }
        private string _customer_id { get; set; }
        private string _customer_password { get; set; }
        private string _delete_flag { get; set; }
        private DateTime _Create_date { get; set; }
        private DateTime _Expiration_date { get; set; }

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public string dd_member_account
        {
            get { return _dd_member_account; }
            set { _dd_member_account = value; }
        }

        public string Mode_code
        {
            get { return _Mode_code; }
            set { _Mode_code = value; }
        }

        public string customer_id
        {
            get { return _customer_id; }
            set { _customer_id = value; }
        }
        public string customer_password
        {
            get { return _customer_password; }
            set { _customer_password = value; }
        }
        public string delete_flag
        {
            get { return _delete_flag; }
            set { _delete_flag = value; }
        }
        public DateTime Create_date
        {
            get { return _Create_date; }
            set { _Create_date = value; }
        }
        public DateTime Expiration_date
        {
            get { return _Expiration_date; }
            set { _Expiration_date = value; }
        }

        #region[File IDataReader]
        public SharingInfo CustomerIDataReader(IDataReader dr)
        {
            SharingInfo obj = new SharingInfo();
            obj.ID = Convert.ToInt32(dr["ID"]);
            obj.dd_member_account = (dr["dd_member_account"] is DBNull) ? string.Empty : dr["dd_member_account"].ToString();
            obj.Mode_code = (dr["Mode_code"] is DBNull) ? string.Empty : dr["Mode_code"].ToString();
            obj.customer_id = (dr["customer_id"] is DBNull) ? string.Empty : dr["customer_id"].ToString();
            obj.customer_password = (dr["customer_password"] is DBNull) ? string.Empty : dr["customer_password"].ToString();
            obj.delete_flag = (dr["delete_flag"] is DBNull) ? string.Empty : dr["delete_flag"].ToString();
            obj.Create_date = Convert.ToDateTime(dr["Create_date"]);
            obj.Expiration_date = Convert.ToDateTime(dr["Expiration_date"]);
            return obj;
        }
        #endregion
    }
    #endregion

    #region SharingControl
    public class SharingControl
    {
        #region[File_GetByAll]
        public List<SharingInfo> Sharing_GetByAll()
        {
            List<SharingInfo> list = new List<SharingInfo>();
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand dbCmd = new SqlCommand("DDV_GetSharingInfo", Conn))
                    {
                        dbCmd.CommandType = CommandType.StoredProcedure;
                        SharingInfo obj = new SharingInfo();
                        SqlDataReader dr = dbCmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                list.Add(obj.CustomerIDataReader(dr));
                            }
                        }
                        dr.Close();
                        obj = null;
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
    }
    #endregion

    #region SharingCallControl
    public class Sharing
    {
        private static SharingControl db = new SharingControl();
        //Customer get by all
        public static List<SharingInfo> Sharing_GetByAll()
        {
            return db.Sharing_GetByAll();
        }
    }
    #endregion
}
