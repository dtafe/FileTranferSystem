<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Site.Master" AutoEventWireup="true" CodeBehind="ManagerMember.aspx.cs" Inherits="FTSS.Web.Form.ManagerMember" %>
<%@ MasterType VirtualPath="~/Form/Site.Master" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../CSS/grid.css" rel="stylesheet" />
    <link href="../CSS/simplePagination.css" rel="stylesheet" />
    <style type="text/css">
        
        .auto-style7
        {
            width: 234px;
        }

        .auto-style10
        {
            width: 234px;
            height: 29px;
        }

        .auto-style11
        {
            width: 151px;
        }

        .auto-style12
        {
            height: 29px;
        }
        .auto-style13
        {
            width: 233px;
            height: 24px;
        }
        .auto-style14
        {
            width: 234px;
            height: 24px;
        }
        .auto-style15
        {
            width: 233px;
            height: 34px;
        }
        .auto-style16
        {
            width: 234px;
            height: 34px;
        }
        .auto-style17
        {
            width: 27px;
        }
        .auto-style5 {
            width: 233px;
            height: 21px;
        }
        .auto-style8 {
            height: 21px;
            width: 219px;
        }
        .auto-style22
        {
            width: 233px;
            height: 29px;
        }
        .auto-style23
        {
            width: 233px;
        }
        .auto-style24
        {
            width: 219px;
            height: 29px;
        }
        .auto-style25
        {
            width: 219px;
        }
        .auto-style26
        {
            width: 224px;
            height: 29px;
        }
        .auto-style27
        {
            width: 224px;
            height: 24px;
        }
        .auto-style28
        {
            width: 224px;
            height: 34px;
        }
        .auto-style29
        {
            width: 224px;
        }
        </style>
    <asp:Panel ID="PanelGridManager" runat="server">
        <table style="width: 100%;">
            <tr>
                <td colspan="3" style="text-align: center; color: #003300; font-size: 18px;" class="auto-style12">
                    <asp:Label ID="LabelManagement" runat="server" Text="Management member"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style22" style="text-align: right">
                    <asp:Label ID="LabelAccount" runat="server" Text="Account"></asp:Label>
                </td>
                <td class="auto-style26">
                    <asp:TextBox ID="TextBoxAccount" runat="server" Width="200px"></asp:TextBox>
                </td>
                <td rowspan="4" class="auto-style11">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style13" style="text-align: right">
                    <asp:Label ID="LabelEmail" runat="server" Text="Email"></asp:Label>
                </td>
                <td class="auto-style27">
                    <asp:TextBox ID="TextBoxEmail" runat="server" Width="200px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="auto-style15" style="text-align: right">
                    <asp:Label ID="LabelRole" runat="server" Text="Role"></asp:Label>
                </td>
                <td class="auto-style28">
                    <asp:DropDownList ID="DropDownListRole" runat="server" Height="21px" Width="204px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="auto-style23">&nbsp;</td>
                <td class="auto-style29">
                    <asp:Button ID="ButtonSearch" runat="server" Text="Search" OnClick="ButtonSearch_Click" Height="25px" />
                    <asp:Button ID="ButtonClear" runat="server" OnClick="ButtonClear_Click" Text="Clear" Height="25px" />
                </td>
                <td>&nbsp;</td>
            </tr>
        </table>
        <asp:GridView ID="GridViewMember" runat="server" CssClass="gridView" AllowPaging="True" AutoGenerateColumns="False" OnPageIndexChanging="GridViewMember_PageIndexChanging" Width="100%" ForeColor="#003300" OnRowCreated="GridViewMember_RowCreated" ShowHeader="False" DataKeyNames="Account">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkStatus" runat="server" AutoPostBack="false"/>
                    </ItemTemplate>                    
                </asp:TemplateField>
                <asp:BoundField DataField="Account"></asp:BoundField>
                <asp:BoundField DataField="Name" />
                <asp:BoundField DataField="Email" />
                <%--<asp:BoundField DataField="Pw" />   --%>              
                <asp:BoundField DataField="Permission" />
            </Columns>
            <PagerStyle CssClass="pagination" BackColor="White" />
            <RowStyle CssClass="tr_body" />
        </asp:GridView>
        <table>
        <tr>
            <td>
                <asp:Button ID="ButtonRegister" runat="server" OnClick="ButtonRegister_Click" Text="Register" Height="21px" style="height: 26px" />
            </td>
            <td class="auto-style17">
                <asp:Button ID="ButtonEdit" runat="server" Text="Edit" Height="25px" OnClick="ButtonEdit_Click" Width="61px" />
            </td>
        </tr>
    </table>
    </asp:Panel>
    
    <asp:Panel ID="PanelMember" runat="server">
        <table style="width: 100%;">
            <tr>
                <td colspan="3" style="text-align: center; color: #003300; font-size: 18px;">
                    <asp:Label ID="LabelEdit" runat="server" Text="Edit member"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style22" style="text-align: right">
                    <asp:Label ID="lblAccount" runat="server" Text="Account"></asp:Label>
                </td>
                <td class="auto-style24">
                    <asp:TextBox ID="txtAccount" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtAccount" ErrorMessage="Please enter Account!" ForeColor="#FF3300" ValidationGroup="ValidationGroupSave" Display="Dynamic">*</asp:RequiredFieldValidator>
                </td>
                <td rowspan="5">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ForeColor="Red" ValidationGroup="ValidationGroupSave" />
                </td>
            </tr>
            <tr>
                <td class="auto-style23" style="text-align: right">
                    <asp:Label ID="lblName" runat="server" Text="Name"></asp:Label>
                </td>
                <td class="auto-style25">
                    <asp:TextBox ID="txtName" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtName" ErrorMessage="Please enter name" ForeColor="#FF3300" ValidationGroup="ValidationGroupSave" Display="Dynamic">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="auto-style22" style="text-align: right">
                    <asp:Label ID="lblEmail" runat="server" Text="Email"></asp:Label>
                </td>
                <td class="auto-style24">
                    <asp:TextBox ID="txtEmail" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtEmail" ErrorMessage="Please enter email!" ForeColor="#FF3300" ValidationGroup="ValidationGroupSave" Display="Dynamic">*</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEmail" ErrorMessage="Please enter the email format!" ForeColor="#FF3300" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="ValidationGroupSave" Display="Dynamic">*</asp:RegularExpressionValidator>
                </td>
            </tr>
            
            <tr>
                <td class="auto-style22" style="text-align: right">
                    <asp:Label ID="lblRole" runat="server" Text="Role"></asp:Label>
                </td>
                <td class="auto-style24">
                    <asp:DropDownList ID="DropDownListAdmin" runat="server" Height="21px" Width="204px">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="DropDownListAdmin" ErrorMessage="Please choose role!" ForeColor="#FF3300" ValidationGroup="ValidationGroupSave" Display="Dynamic">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="auto-style5" style="text-align: right">
                    <asp:Label ID="lblPassword" runat="server" Text="Password"></asp:Label>
                </td>
                <td class="auto-style8">
                    <asp:TextBox ID="txtPassword" runat="server" Width="200px" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtPassword" ErrorMessage="Please enter password!" ForeColor="#FF3300" ValidationGroup="ValidationGroupSave" Display="Dynamic">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="auto-style23">&nbsp;</td>
                <td class="auto-style25">
                    <asp:Button ID="ButtonSave" runat="server" OnClick="ButtonSave_Click" Text="Save" ValidationGroup="ValidationGroupSave" Height="23px" />
                    <asp:Button ID="ButtonDelete" runat="server" Text="Delete" OnClick="ButtonDelete_Click" Height="23px" />
                    <asp:Button ID="ButtonCancel" runat="server" OnClick="ButtonCancel_Click" Text="Cancel" Height="23px" />
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style23">&nbsp;</td>
                <td class="auto-style25">
                    <asp:Label ID="LabelMessage" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                </td>
                <td>&nbsp;</td>
            </tr>
        </table>
        
    </asp:Panel>
</asp:Content>
