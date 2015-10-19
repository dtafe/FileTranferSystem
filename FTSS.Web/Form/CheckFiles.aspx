<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Site.Master" AutoEventWireup="true" CodeBehind="CheckFiles.aspx.cs" Inherits="FTSS.Web.Form.CheckFiles" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <style type="text/css">
        .auto-style2 {
            height: 20px;
        }
    </style>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:Label ID="LabelTitle" runat="server" Text="File Transfer and Sharing System" Visible="False"></asp:Label>

        <asp:Label ID="LabelCustomerName" runat="server"  Visible="false" ></asp:Label>      
        <br />
        <table align="center">
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>

      
        <asp:Label ID="LabelInvitationTo" runat="server" Text="Invitation to" Font-Bold="True"></asp:Label>

                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
 
        <asp:GridView DataKeyNames="CustomerEmail" ID="gridCustomers" runat="server"
                PagerStyle-HorizontalAlign="Center" AutoGenerateColumns="False" Width="400px"
                CellPadding="0" BorderWidth="0px" GridLines="None" ShowHeader="False" style="text-align: left">
                <AlternatingRowStyle CssClass="GridAlternate" />
                <PagerStyle HorizontalAlign="Center"></PagerStyle>
                <RowStyle CssClass="GridNormalRow" />
                                                     
               <Columns>
                     <asp:BoundField DataField="CustomerName" HeaderStyle-Width="0px" ItemStyle-Width="150px" >
                            <HeaderStyle Width="0px"></HeaderStyle>
                            <ItemStyle Width="150px"></ItemStyle>
                     </asp:BoundField>
                     <asp:BoundField DataField = "CustomerEmail" ItemStyle-Width="250px" >
                            <ItemStyle Width="250px"></ItemStyle>
                     </asp:BoundField>
               </Columns>
                    <EmptyDataRowStyle CssClass="GridEmptyRow" />
                    <EmptyDataTemplate>
                        <span>No customers added</span>
                    </EmptyDataTemplate>
        </asp:GridView>
         
    
                </td>
            </tr>
            <tr>
                <td class="auto-style2">
        <asp:Label ID="LabelFiles" runat="server" Text="Files:" Font-Bold="True"></asp:Label>
         
    
                </td>
                <td class="auto-style2"></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
         
    
<asp:GridView ID="fileGrid" AutoGenerateColumns="False" ShowHeader="False" BorderColor="#9ABBE8" 
            BorderWidth="0px" runat="server" OnRowCommand="fileGrid_RowCommand"  OnRowDataBound="fileGrid_RowDataBound"  GridLines="None" style="text-align: left" OnRowDeleting="fileGrid_RowDeleting"> 
            <Columns> 
               <asp:TemplateField ItemStyle-HorizontalAlign="Center" >  
                     <ItemTemplate>  
                            <%#Container.DataItemIndex + 1 %> .  
                     </ItemTemplate>  

                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:TemplateField>  
                <asp:BoundField DataField="ID"  ItemStyle-Width="0px" ItemStyle-ForeColor="White" >
                        <ItemStyle ForeColor="White" Width="0px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="File_Name"  ItemStyle-Width="200px" >
                        <ItemStyle Width="220px"></ItemStyle>
                </asp:BoundField>
                 <asp:TemplateField ShowHeader="False" ItemStyle-Width="70px">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButtonDownload" runat="server" CausesValidation="False" CommandName="Download"  Text='<%# GetResource("ButtonDownload")%>' ></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>     
                 <asp:TemplateField ShowHeader="False" ItemStyle-Width="70px">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButtonDelete" runat="server" CausesValidation="False" CommandName="Delete"  Text='<%# GetResource("ButtonLinkDelete")%>' ></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>     
               <%-- <asp:ButtonField ButtonType="Button" Text="Download"  CommandName="Download"/>
                <asp:ButtonField ButtonType="Button" Text="Delete"  CommandName="Delete"/>--%>

            </Columns> 
        </asp:GridView> 

                </td>
            </tr>
            <tr>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td></td>
                <td> 

    <asp:Button ID="ButtonAllFilesDownload" runat="server" Text="All Files Download" OnClick="ButtonAllFilesDownload_Click" />
    <asp:Button ID="ButtonAllFilesDelete" runat="server" Text="All Files Delete" OnClick="ButtonAllFilesDelete_Click" />
    
                </td>
            </tr>
            <tr>
                <td>
    
    <asp:Label ID="lbExpirationDate" runat="server" Text="Expiration Date" Font-Bold="True"></asp:Label>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
    <asp:Label ID="lbDate" runat="server" Text=""></asp:Label>

                </td>
            </tr>
            <tr>
                <td>
                    <asp:HiddenField ID="HiddenFieldCustomerID" runat="server" />
                </td>
                <td style="padding-left:40px"> 
                    <asp:Button ID="ButtonBack" runat="server" Text="Back" OnClick="ButtonBack_Click" style="margin-right:10px" />
                    <asp:Button ID="ButtonClose" runat="server" Text="Close" OnClick="ButtonClose_Click" /> 
                </td>
            </tr>
        </table>
     <asp:Label ID="fileId" runat="server"  Font-Size="XX-Small" ForeColor="White"/> 
</asp:Content>
