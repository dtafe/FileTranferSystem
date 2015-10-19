<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Site.Master" AutoEventWireup="true" CodeBehind="Use.aspx.cs" Inherits="FTSS.Web.Form.Use" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 
<style type="text/css">
.ScrollStyle
{
    max-height: 800px;
    overflow-y: scroll;
}
    .InfoStyle
    {
        padding-left:450px;
    }
</style>

<div>
        <div>
            <p> 1. If you have not member account Login FTSS , please contact to admin for creating new account 
          or you can <asp:HyperLink ID="Link1" runat="server" NavigateUrl="~/Form/RegisterMember.aspx" Text="Register here" /> &nbsp;. And wait confirmed by admin system.
             </p>
            <asp:Label runat="server"  ID="lblEmail"  CssClass="InfoStyle" Text ="Email Address : ftss@datadesign.co.jp "></asp:Label>
            <br />
            <asp:Label ID="lblTEL" runat="server" CssClass="InfoStyle" Text ="TEL : +81-52-953-1588"></asp:Label>
            <br />
              <asp:Label ID="lblFAX" runat="server" CssClass="InfoStyle" Text ="FAX : +81-045-478-0581"></asp:Label>
        </div>
     
     <p> 2. Flow Chart of FTSS </p>
     <asp:Image ID="imgFlowUse" runat="server" ImageUrl="~/Images/flowuse.jpg" Width="680px" />
     <br />

    <div style="text-align:right">
       <asp:Button ID="ButtonClose" runat="server" Text="Close" OnClick="ButtonClose_Click" Style ="margin-right:20px"    />
    </div>
 
    <br />
</div>

   

</asp:Content>
