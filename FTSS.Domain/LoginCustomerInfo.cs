using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTSS.Domain
{
    public class LoginCustomerInfo
    {
        private string _customer_id;
        private string _customer_password;
        private int _Mode_code;
        private string _delete_flag;
        private DateTime _Expiration_date;

        #region Propertise
        public string Customer_id
        {
            get { return _customer_id; }
            set { _customer_id = value; }
        }
        public string Customer_password
        {
            get { return _customer_password; }
            set { _customer_password = value; }
        }

        public int Mode_code
        {
            get { return _Mode_code; }
            set { _Mode_code = value; }
        }
        public string Delete_flag
        {
            get { return _delete_flag; }
            set { _delete_flag = value; }
        }
        public DateTime Expiration_date
        {
            get { return _Expiration_date; }
            set { _Expiration_date = value; }
        }
        #endregion
    }
}
