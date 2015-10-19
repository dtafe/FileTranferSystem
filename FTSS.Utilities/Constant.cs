using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTSS.Utilities
{
    public class Constant
    {
        #region Mail information
        public const string MAIL_SERVER = "mail.mailServer";
        public const string MAIL_USER_NAME = "mail.mailUserName";
        public const string MAIL_USER = "mail.mailUser";
        public const string MAIL_PWD = "mail.mailPwd";
        public const string MAIL_TIME_OUT = "mail.mailTimeOut";
        public const string MAIL_PORT = "mail.port";
        public const string MAIL_ENABLE_SSL = "mail.EnableSsl";
        #endregion

        public const string FTSS_CONNECT_STRING = "FTSSConnectionString";
        public const string MAIL_TEMPLATE = "MailTemplate";
        public const string UPLOAD_STORAGE = "Upload";

        public const string PARAM_CUSTOMER_ID = "customerId";
        public const string PORTAL_URL = "UrlPortal";
        public const string STORAGE_CONNECT_STRING =  "StorageConnectionString";
        public const string STORAGE_CONTAINER_NAME = "BlobContainer";

        public const string MAIL_DELIVERY_ADDRESS = "mailGridDelivery.mailAddress";
        public const string MAIL_DELIVERY_DISPLAY = "mailGridDelivery.displayName";
        public const string MAIL_DELIVERY_USER = "mailGridDelivery.mailUserName";
        public const string MAIL_DELIVERY_PASS = "mailGridDelivery.mailPwd";

        public const int MAX_FILE_COMPARE = 1024 * 1024 * 200;  //200MB
    }
}
