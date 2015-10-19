using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTSS.Domain
{
    public class CheckOwnFilesInfo
    {
        public int ID { get; set; }
        public DateTime Create_date { get; set; }
        public string Customer_name { get; set; }
        public string Customer_Email { get; set; }
        public int Mode_code { get; set; }
        public string File_name { get; set; }
        public string File_size { get; set; }
        public DateTime Expiration_date { get; set; }
        public string delete_flag { get; set; }
    }
}
