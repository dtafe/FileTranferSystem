<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Site.Master" AutoEventWireup="true" CodeBehind="LoginMember.aspx.cs" Inherits="FTSS.Web.Form.LoginMember" %>
<%@ MasterType VirtualPath="~/Form/Site.Master" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <script>
    function EnterEvent(e) {
        if (e.keyCode == 13) {
            //__doPostBack('<%=ButtonLogin.UniqueID%>', "");
            //$("#ButtonLogin").trigger("OnClick");
        }
    }
        function CloseWinDow()
        {
            window.close();
        }
</script>
    <asp:Panel runat="server" DefaultButton="ButtonLogin">
        <table align="center">
             <tr>
                <td>
                     <asp:ValidationSummary ID="LoginMem" runat="server" ForeColor="Red" ValidationGroup="LoginMem" Style="text-align:left" Width="382px" />
                </td>
            </tr>
        </table>

       <table align="center">          
        <tr>
            <td>
                <asp:Label ID="LabelUserAccount" runat="server" Text="User Account"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtUser" runat="server" Width="200px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="requiredUser" runat="server" ControlToValidate="txtUser" 
                                               ForeColor="Red" ValidationGroup="LoginMem" Display="Dynamic">*</asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="LabelPassword" runat="server" Text="Password"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtPassword" runat="server" ClientIDMode="Static" TextMode="Password" onkeypress="return EnterEvent(event)" Width="200px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="requiredPassword" runat="server" ControlToValidate="txtPassword" 
                                               ForeColor="Red" ValidationGroup="LoginMem" Display="Dynamic">*</asp:RequiredFieldValidator>
             </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lbMode" runat="server" Text="Mode Select"></asp:Label>
            </td>
            <td>
            </td>
        </tr>

        <tr>
            <td> 
            </td>
            <td>
                <asp:RadioButton ID="rdbDownloadFile" runat="server"  Text="Customer download a file (Upload files)" GroupName="SelectMode" Checked="True" />
            </td>
        </tr>

        <tr>
            <td> 
            </td>
            <td>
                <asp:RadioButton ID="rdbIssueToCustomer" runat="server"  Text="Issue of upload ID for customer" GroupName="SelectMode" />
            </td>
        </tr>

        <tr>
            <td> 
            </td>
            <td>
                <asp:RadioButton ID="rdbCheckOwnFile" runat="server"  Text="Check own sharing files" GroupName="SelectMode" />
            </td>
        </tr>

        <tr>
            <td>
            </td>
            <td style="padding-left:20px;padding-top:8px;">
                <asp:Button ID="ButtonLogin" runat="server" ClientIDMode="Static" Text="Login" style="margin-right: 10px" OnClick="cmdLogin_Click" ValidationGroup="LoginMem"  />
                <asp:Button ID="ButtonCancel" runat="server" Text="Cancel" OnClientClick="CloseWinDow();" />
            </td>
        </tr>

        <tr> 
          <td class="auto-style4"></td>
          <td class="auto-style4">
             <asp:LinkButton ID="LinkButtonUse" runat="server" PostBackUrl="~/Form/Use.aspx">How to use</asp:LinkButton>
                 | 
             <asp:LinkButton ID="LinkButtonAdmin" runat="server" OnClick="LinkButtonAdmin_Click">Management Member</asp:LinkButton>
            </td>
      </tr>
    </table>
  </asp:Panel>
</asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .auto-style4
        {
            height: 21px;
        }
    </style>
</asp:Content>

