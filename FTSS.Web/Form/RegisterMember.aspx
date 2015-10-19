<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Site.Master" AutoEventWireup="true" CodeBehind="RegisterMember.aspx.cs" Inherits="FTSS.Web.Form.RegisterMember" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">

        
        .auto-style4
        {
            width: 229px;
        }
        .auto-style5
        {
            width: 229px;
            height: 29px;
        }
        .auto-style6
        {
            height: 29px;
        }

        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="width:100%;">
        <tr>
            <td colspan="2" style="text-align: center; font-weight: bold;">
                <asp:Label ID="LabelRegister" runat="server" ForeColor="#003300" Text="Register Account Infomation"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="auto-style4" style="text-align: right">
                <asp:Label ID="LabelAccount" runat="server" Text="Account"></asp:Label>
                :</td>
            <td class="auto-style6">
                <asp:TextBox ID="TextboxAccount" runat="server" Width="220px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextboxAccount" ErrorMessage="Please enter Account!" ForeColor="Red" ValidationGroup="ValidationGroupRegister">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style4" style="text-align: right">
                <asp:Label ID="LabelName" runat="server" Text="Name"></asp:Label>
                :</td>
            <td class="auto-style6">
                <asp:TextBox ID="TextboxName" runat="server" Width="220px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextboxName" ErrorMessage="Please enter name!" ForeColor="Red" ValidationGroup="ValidationGroupRegister">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style4" style="text-align: right">
                <asp:Label ID="LabelPassword" runat="server" Text="Password"></asp:Label>
                :</td>
            <td>
                <asp:TextBox ID="TextboxPassword" runat="server" TextMode="Password" Width="220px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextboxPassword" ErrorMessage="Please enter Password!" ForeColor="Red" ValidationGroup="ValidationGroupRegister">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style4" style="text-align: right">
                <asp:Label ID="LabelRePassword" runat="server" Text="Re-password"></asp:Label>
                :</td>
            <td>
                <asp:TextBox ID="TextboxRePassword" runat="server" TextMode="Password" Width="220px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextboxRePassword" ErrorMessage="Please enter Re-password" ForeColor="Red" ValidationGroup="ValidationGroupRegister">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="TextboxPassword" ControlToValidate="TextboxRePassword" ErrorMessage="Password does not match!" ForeColor="Red" ValidationGroup="ValidationGroupRegister">*</asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style5" style="text-align: right">
                <asp:Label ID="LabelEmail" runat="server" Text="Email"></asp:Label>
                :</td>
            <td class="auto-style6">
                <asp:TextBox ID="TextboxEmail" runat="server" Width="220px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Please enter Email! " ForeColor="Red" ControlToValidate="TextboxEmail" ValidationGroup="ValidationGroupRegister">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TextboxEmail" ErrorMessage="Please enter the correct format email!" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ForeColor="Red" ValidationGroup="ValidationGroupRegister">*</asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style4" style="text-align: right"></td>
            <td class="auto-style8">
                <asp:Button ID="ButtonRegister" runat="server" OnClick="buttonRegister_Click" Text="Register" Width="82px" Height="23px" ValidationGroup="ValidationGroupRegister" />
                <asp:Button ID="ButtonCancel" runat="server" Text="Cancel" OnClick="ButtonCancel_Click" Height="23px" />
            </td>
        </tr>
        <tr>
            <td class="auto-style4" style="text-align: right">&nbsp;</td>
            <td>
                <asp:ValidationSummary ID="ValidationSummary1" ForeColor="red" runat="server" HeaderText="Validation Error" style="margin-left: 0px" ValidationGroup="ValidationGroupRegister" />
            </td>
        </tr>
    </table>
</asp:Content>
