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
    #region CustomerInfo
    [Serializable]
    public class CustomerInfo
    {
        private string _File_SharingID;
        private string _Customer_name;
        private string _Customer_Email;
        public string CustomerId { get; set; }

        public CustomerInfo()
        {
        }
        public CustomerInfo(string name, string email)
        {
            _Customer_name = name;
            _Customer_Email = email;
        }
        public string File_Sharing_ID
        {
            get { return _File_SharingID; }
            set { _File_SharingID = value; }
        }
        public string CustomerName
        {
            get { return _Customer_name; }
            set { _Customer_name = value; }
        }
        public string CustomerEmail
        {
            get { return _Customer_Email; }
            set { _Customer_Email = value; }
        }

        #region[File IDataReader]
        public CustomerInfo CustomerIDataReader(IDataReader dr)
        {
            CustomerInfo obj = new CustomerInfo();
            obj.File_Sharing_ID = (dr["File_Sharing_ID"] is DBNull) ? string.Empty : dr["File_Sharing_ID"].ToString();
            obj.CustomerName = (dr["Customer_name"] is DBNull) ? string.Empty : dr["Customer_name"].ToString();
            obj.CustomerEmail = (dr["Customer_Email"] is DBNull) ? string.Empty : dr["Customer_Email"].ToString();
            return obj;
        }
        #endregion
    }
    #endregion
    #region CustomerControl
    public class CustomerControl
    {
        #region[File_GetByAll]
        public List<CustomerInfo> File_GetByAll()
        {
            List<CustomerInfo> list = new List<CustomerInfo>();
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand cmd = new SqlCommand("DDV_GetCustomerInfo", Conn))
                    {
                        CustomerInfo obj = new CustomerInfo();
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
    }
    #endregion
    #region CustomerCallControl
    public class CustomerCallControl
    {
        private static CustomerControl db = new CustomerControl();
        //Customer get by all
        public static List<CustomerInfo> File_GetByAll()
        {
            return db.File_GetByAll();
        }
    }
    #endregion    
}
