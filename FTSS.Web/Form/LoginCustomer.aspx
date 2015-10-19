<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Site.Master" AutoEventWireup="true" CodeBehind="LoginCustomer.aspx.cs" Inherits="FTSS.Web.Form.LoginCustomer" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="pal" runat="server" DefaultButton="ButtonLogin">
        <div style="text-align:center">
            <asp:Label ID="LabelTitleCustomerLogin" runat="server"></asp:Label>
        </div>
        <br />
        <table align="center" style="text-align:center; width: 329px;">
                 <tr>
                    <td class="auto-style5">
                        <asp:ValidationSummary ID="LoginCustom" runat="server" ForeColor="Red" ValidationGroup="checkLogin" Style="text-align:left" Width="253px"/>
                    </td>
                </tr>
            </table>
        <div>            
            <table align="center" class="auto">               
                <tr>
                    <td>
                        <asp:Label ID="LabelLoginID" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtLoginCustomerID" runat="server" Width="180px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="Val_RequiredLoginID" runat="server" ControlToValidate="txtLoginCustomerID" Display="Dynamic" ForeColor="Red" ValidationGroup="checkLogin">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="LabelPassword" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtLoginCustomerPass" runat="server" TextMode="Password" Width="180px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="Val_RequiredLoginPass" runat="server" ControlToValidate="txtLoginCustomerPass" Display="Dynamic" ForeColor="Red" ValidationGroup="checkLogin">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td style="text-align: right">
                        <asp:Button ID="ButtonLogin" runat="server" OnClick="ButtonLogin_Click" ValidationGroup="checkLogin" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .auto-style5 {
            width: 300px;
        }
    </style>
</asp:Content>

