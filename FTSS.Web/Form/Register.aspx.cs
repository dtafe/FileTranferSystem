using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FTSS.Web.Form
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonClose_Click(object sender, EventArgs e)
        {
            Server.Transfer("LoginMember.aspx"); 
        }
    }
}