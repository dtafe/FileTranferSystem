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
    #region DDMemberInfoCustomer
    [Serializable]
    public class DDMemberInfoCustomer
    {
        private string _Account;
        private string _Name;
        private string _Email;
        private string _Pw;

        public string Account
        {
            get { return _Account; }
            set { _Account = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        public string Pw
        {
            get { return _Pw; }
            set { _Pw = value; }
        }

        #region[File IDataReader]
        public DDMemberInfoCustomer CustomerIDataReader(IDataReader dr)
        {
            DDMemberInfoCustomer obj = new DDMemberInfoCustomer();
            obj.Account = (dr["Account"] is DBNull) ? string.Empty : dr["Account"].ToString();
            obj.Name = (dr["Name"] is DBNull) ? string.Empty : dr["Name"].ToString();
            obj.Email = (dr["Email"] is DBNull) ? string.Empty : dr["Email"].ToString();
            obj.Pw = (dr["Pw"] is DBNull) ? string.Empty : dr["Pw"].ToString();
            return obj;
        }
        #endregion
    }
    #endregion
    #region DDMemberControlCustomer
    public class DDMemberControlCustomer
    {
        #region[File_GetByAll]
        public List<DDMemberInfoCustomer> File_GetByAll()
        {
            List<DDMemberInfoCustomer> list = new List<DDMemberInfoCustomer>();
            using (SqlConnection Conn = new SqlConnection(Common.GetConnectString()))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand dbCmd = new SqlCommand("DDV_GetMemberInfo", Conn))
                    {
                        DDMemberInfoCustomer obj = new DDMemberInfoCustomer();
                        dbCmd.CommandType = CommandType.StoredProcedure;
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
    #region DDMemberCallControlCustomer
    public class DDMemberCallControlCustomer
    {
        private static DDMemberControlCustomer db = new DDMemberControlCustomer();
        //Customer get by all
        public static List<DDMemberInfoCustomer> File_GetByAll()
        {
            return db.File_GetByAll();
        }
    }
    #endregion
}
