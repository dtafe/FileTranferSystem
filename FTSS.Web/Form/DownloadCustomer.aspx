<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Site.Master" AutoEventWireup="true" CodeBehind="DownloadCustomer.aspx.cs" Inherits="FTSS.Web.Form.DownloadCustomer" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <table align = "center">
        <tr>
            <td>
    <div>
        <script type="text/javascript">
            function CheckAll() {
                var checkall = document.getElementById('<%=CheckBoxAll.ClientID%>');  
                var checklist = document.getElementById('<%=CheckBoxListFile.ClientID%>');
                var check = checklist.getElementsByTagName('input');
                if (checkall.checked) {
                    for (var i = 0; i < check.length ; i++) {
                        check[i].checked = true;
                    }
                }
                else {
                    for (var i = 0; i < check.length ; i++) {
                        check[i].checked = false;
                    }
                }
            }
            function Cheklist() {
                var checkall = document.getElementById('<%=CheckBoxAll.ClientID%>');
                var checklist = document.getElementById('<%=CheckBoxListFile.ClientID%>');
                var check = checklist.getElementsByTagName('input');
                var total = 0;
                for (var i = 0; i < check.length ; i++) {
                    if (check[i].checked)
                        total++;
                }
                if (total == check.length)
                    checkall.checked = true;
                else checkall.checked = false;
            }
            function CloseWindow() {
                //case OpenPopUp
                window.close();
            }
        </script>
    <table >
        <tr>
            <td>
                <asp:Label ID="LabelWelcome" runat="server" Text="Welcome---"></asp:Label></td>
            <td>
                <asp:Label ID="LabelCustomerName" runat="server" ></asp:Label></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="LabelFileDownloadPeriod" runat="server" Text="File download period--- "></asp:Label></td>
            <td>
                <asp:Label ID="LabelDate" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="LabelFile" runat="server" Text="File---"></asp:Label></td>
            <td></td>                
        </tr>
        </table>                  
           <asp:CheckBox ID="CheckBoxAll" runat="server" AutoPostBack="false" onclick="CheckAll()"/>
           <asp:CheckBoxList ID="CheckBoxListFile" runat="server" AutoPostBack="false" onclick="Cheklist()" Style="padding-left:10px" ></asp:CheckBoxList>
             <asp:Button ID="ButtonDownload" runat="server" Text="Download" OnClick="ButtonDownload_Click" />    
            <asp:Button ID="ButtonClose" runat="server" Text="Button" OnClick="ButtonClose_Click" />          
        <asp:HiddenField ID="HiddenFieldAcount" runat="server" /> 
   </div>                    
            </td>
        </tr>
    </table>   
</asp:Content>
