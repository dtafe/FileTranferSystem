<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Site.Master" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="FTSS.Web.Form.Upload" %>


<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../Script/jquery-1.11.3.min.js"></script>
    <link href="../CSS/fileUpload.css" rel="stylesheet" />
    <script src="../Script/fileUpload.js" type="text/javascript" lang="javascript"></script>
    <link href="../CSS/ThemeBlue.css" rel="Stylesheet" type="text/css" />
    <style type="text/css">
        div.fileinputs
        {
            position: relative;
        }

        div.fakefile
        {
            position: absolute;
            top: 0px;
            left: 0px;
            z-index: 1;
            height: 31px;
            width: 307px;
        }

        input.file
        {
            position: relative;
            text-align: right;
            -moz-opacity: 0;
            filter: alpha(opacity: 0);
            opacity: 0;
            z-index: 2;
        }

        .styleFile
        {
            width: 305px;
        }

        .auto-style4
        {
            width: 666px;
        }
    </style>
    <script type="text/javascript">
        <%--function SetFileGridIndex(index) {
            var myControl = document.getElementById('<%= fileGridIndex.ClientID %>');
            myControl.value = index;
        }--%>
        function ConfirmOnDelete(confirmMessage) {
            if (confirm(confirmMessage) == true)
                return true;
            else
                return false;
        }
        function CloseWindow() {
            //case OpenPopUp
            window.close();
        }
        <%--function change() {
            document.getElementById('<%=txtFilePath.ClientID%>').value = document.getElementById('<%=fileUpload.ClientID%>').value;
        }--%>

    </script>

    <table align="center" style="padding-left: 120px">
        <tr>
            <td class="auto-style4">
                <asp:ValidationSummary ID="RegisterCustomer" runat="server" ForeColor="Red" ValidationGroup="RegisterCustomer" />
                <asp:ValidationSummary ID="RegisterFile" runat="server" ForeColor="Red" ValidationGroup="RegisterFile" />
                <br />

                <asp:ScriptManager ID="ScriptManager1" runat="server">
                </asp:ScriptManager>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="LabelCustomerInfo" runat="server" Style="margin-bottom: 5px;" Font-Bold="True"></asp:Label>
                        <table cellspacing="5" class="Container" border="0" style="padding-top: 5px; padding-left: 5px; padding-bottom: 5px; padding-right: 5px; margin-top: 5px; margin-left: 5px">
                            <tr style="padding-top: 10; padding-left: 5px; padding-bottom: 5px; padding-right: 5px; margin-top: 5px">
                                <td>
                                    <asp:Label ID="LabelCustomerName" runat="server"></asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtName" runat="server" Height="20px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="requiredName" runat="server" ControlToValidate="txtName"
                                        ForeColor="Red" ValidationGroup="RegisterCustomer">*</asp:RequiredFieldValidator>
                                </td>

                            </tr>
                            <tr style="padding-top: 10px; padding-left: 5px; padding-bottom: 5px; padding-right: 5px; margin-top: 5px">
                                <td>
                                    <asp:Label ID="LabelEmailAddress" runat="server"></asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtEmail" runat="server" Width="266px" Height="20px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="requiredEmail" runat="server" ControlToValidate="txtEmail"
                                        ForeColor="Red" ValidationGroup="RegisterCustomer" Display="Dynamic">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="validateEmail" runat="server" ControlToValidate="txtEmail"
                                        ValidationExpression="^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$" ValidationGroup="RegisterCustomer" ForeColor="Red" Display="Dynamic">*</asp:RegularExpressionValidator>
                                </td>
                                <td>
                                    <asp:Button ID="ButtonAddCustomer" runat="server"
                                        ValidationGroup="RegisterCustomer" OnClick="btnAddCustomer_Click" Style="Height: 20px; min-width: 60px;" />
                                </td>
                            </tr>
                        </table>
                        <div style="padding-top: 5px">
                            <asp:Label ID="LabelCustomerList" runat="server" Font-Bold="True"></asp:Label>
                        </div>
                        <asp:GridView DataKeyNames="CustomerEmail" ID="gridCustomers" AllowPaging="false" runat="server"
                            PagerStyle-HorizontalAlign="Center" AutoGenerateColumns="false" Width="400px"
                            CellPadding="0" BorderWidth="0" GridLines="None" ShowHeader="false"
                            OnRowDataBound="gridCustomers_RowDataBound" OnRowDeleting="gridCustomers_RowDeleting">
                            <AlternatingRowStyle CssClass="GridAlternate" />
                            <PagerStyle HorizontalAlign="Center" />
                            <RowStyle CssClass="GridNormalRow" />
                            <Columns>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex + 1 %>. 
                                    </ItemTemplate>
                                    <ItemStyle Width="50px"></ItemStyle>
                                </asp:TemplateField>
                                <asp:BoundField DataField="CustomerName" HeaderStyle-Width="0px" ItemStyle-Width="150px">
                                    <HeaderStyle Width="0px" />
                                    <ItemStyle Width="150px" />
                                </asp:BoundField>
                                <asp:HyperLinkField DataTextField="CustomerEmail" ItemStyle-Width="250px">
                                    <ItemStyle Width="250px" />
                                </asp:HyperLinkField>
                                <asp:TemplateField ItemStyle-Width="150px" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Delete" Text='<%# GetResource("ButtonLinkDelete")%>'></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="150px" />
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataRowStyle CssClass="GridEmptyRow" />
                            <EmptyDataTemplate>
                                <span>No customers added</span>
                            </EmptyDataTemplate>

                        </asp:GridView>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ButtonAddCustomer" />
                    </Triggers>
                </asp:UpdatePanel>

                <!--Drop drag file upload-->
                <asp:Panel ID="panelUploadFiles" runat="server">
                    
                    <br />
                    <asp:Label ID="LabelbUploadfilelist" runat="server" Font-Bold="True" Style="padding-top: 10px"></asp:Label>
                    <input id="fileInput" type="file" name="fileInput" multiple style="display: none;"/>
                    <div id="status1"></div>
                    <div id="dragandrophandler">
                        <br /><br />
                        <asp:Label ID="lblDropdrag" runat="server" Text="drag and drop or click file upload" ForeColor="#15317E"></asp:Label>
                    </div>
                </asp:Panel>
                <!--End Drop drag file upload-->
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>

                        <table style="padding-top: 10px; padding-left: 10px">
                            <tr>
                                <td>
                                    <asp:Label ID="LabelTermUpload" runat="server"></asp:Label>
                                    <asp:DropDownList ID="dropdownTermUpload" runat="server" Style="margin-left: 20px;" Width="100px" Height="21px">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:Label ID="LabelDay" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <table>
                                        <tr>
                                            <td style="width: 70px"></td>
                                            <td style="width: 40px">
                                                <asp:UpdateProgress ID="UpdWaitImage" runat="server" DynamicLayout="true" AssociatedUpdatePanelID="UpdatePanel1">
                                                    <ProgressTemplate>
                                                        <asp:Image ID="imgProgress" ImageUrl="~/Images/loading.gif" runat="server" />
                                                    </ProgressTemplate>
                                                </asp:UpdateProgress>
                                            </td>
                                            <td>
                                                <div style="padding-top: 10px; text-align: right">
                                                    <asp:Button ID="ButtonUpload" runat="server" Style="Height: 21px; min-width: 60px;" OnClick="btnUpload_Click" />
                                                    <asp:Button ID="ButtonCancel" runat="server" Style="Height: 21px; min-width: 60px;" OnClick="ButtonCancel_Click" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                    </ContentTemplate>
                    
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="fileGridIndex" runat="server" />
    <asp:HiddenField ID="hiddenAccount" Value="" runat="server" />

    </td>
    </table>
                    
    <br />
</asp:Content>
