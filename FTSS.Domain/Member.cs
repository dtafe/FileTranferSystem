using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTSS.Domain
{
    public class Member
    {
        public string Account { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Pw { get; set; }
        public string Permission { get; set; }

        #region[File IDataReader]
        public Member MemberIDataReader(IDataReader dr)
        {
            Member obj = new Member();           
            obj.Account = (dr["Account"] is DBNull) ? string.Empty : dr["Account"].ToString();
            obj.Name = (dr["Name"] is DBNull) ? string.Empty : dr["Name"].ToString();
            obj.Email = (dr["Email"] is DBNull) ? string.Empty : dr["Email"].ToString();
            obj.Pw = (dr["Pw"] is DBNull) ? string.Empty : dr["Pw"].ToString();
            obj.Permission = (dr["Permission"] is DBNull) ? string.Empty : dr["Permission"].ToString(); 
            return obj;
        }
        #endregion
    }

}
