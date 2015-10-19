<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Site.Master" AutoEventWireup="true" CodeBehind="CheckOwnFile.aspx.cs" Inherits="FTSS.Web.Form.CheckOwnFile" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="Stylesheet" href="../CSS/grid.css" />
    <link rel="stylesheet" href="../CSS/simplePagination.css" />
     <script type="text/javascript">

         function rowno(rowindex) {
             var i, CellValue, Row;
             i = parseInt(rowindex) + 1;

             var table = document.getElementById('<%=gridCheckOwnFile.ClientID %>');

            Row = table.rows[i];
            CellValue = Row.cells[0].innerHTML;
            window.location.replace("/Form/CheckFiles.aspx?ID=" + CellValue + "");
            return false;
        }


    </script>
    <div>
        <asp:Label ID="LabelTitleCheckOwnFile" runat="server" Text="Label"></asp:Label>
    </div>
    <div>
        <table align="center" class="auto">
            <tr>
                <td style="text-align: right">
                    <asp:CheckBox ID="CheckBoxShowDelete" runat="server" AutoPostBack="True" OnCheckedChanged="CheckBoxShowDelete_CheckedChanged" />
                    <asp:GridView ID="gridCheckOwnFile" runat="server" CssClass="gridView" AutoGenerateColumns="False" CellPadding="4" Style="text-align: left" OnPreRender="gridCheckOwnFile_PreRender" OnRowCreated="gridCheckOwnFile_RowCreated" ShowHeader="False" OnRowDataBound="gridCheckOwnFile_RowDataBound" AllowPaging="True" OnPageIndexChanging="gridCheckOwnFile_PageIndexChanging" Width="786px">
                        <Columns>

                            <asp:BoundField DataField="ID">
                                <ItemStyle Font-Underline="true" HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Create_date" >
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Customer_name" />
                            <asp:BoundField DataField="Customer_Email" />
                            <asp:BoundField DataField="Mode_code" />
                            <asp:BoundField DataField="File_name" />
                            <asp:BoundField DataField="File_size" />
                            <asp:BoundField DataField="Expiration_date" >
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:BoundField>
                            <asp:BoundField DataField="delete_flag" >
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:BoundField>
                        </Columns>
                        <PagerSettings FirstPageText="First" LastPageText="Last" NextPageText="Next" PreviousPageText="Prev"
                         Mode="NumericFirstLast" PageButtonCount="5" />
                        <PagerStyle CssClass="pagination" BackColor="White" />
                        <RowStyle CssClass="tr_body" />
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td style="text-align: left">
                    <asp:Label ID="LabelNoRecord" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>


