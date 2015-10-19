<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Site.Master" AutoEventWireup="true" CodeBehind="UploadCustomer.aspx.cs" Inherits="FTSS.Web.Form.UploadCustomer" %>
 
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">    

    <style type="text/css">
         div.fileinputs {
	        position: relative;
        }

        div.fakefile {
	        position: absolute;
	        top: 0px;
	        left: 0px;
	        z-index: 1;
            width: 394px;
        }

        input.file {
	        position: relative;
	        text-align: right;
	        -moz-opacity:0;
	        filter:alpha(opacity: 0);
	        opacity: 0;
	        z-index: 2;
            top: 0px;
            left: 0px;
            width: 314px;
        }
        .auto-style4 {
            width: 587px;
        }
    </style>
  <script>       
      function change()
      {
          document.getElementById('<%=txtFilePath.ClientID%>').value = document.getElementById('<%=FileUploadSelect.ClientID%>').value;
      }      
  </script> 
    <%-- <asp:ScriptManager
			ID="ScriptManager1"
			runat="server">
		</asp:ScriptManager>
          <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>--%>
    <table align = "center" style="padding-left:80px">
        <tr>
            <td class="auto-style4">
            
    <asp:Panel ID="pnLoad" runat="server">  
    <table>
        <tr>
            <td >
                <asp:Label ID="LabelWelcome" runat="server" Text=""></asp:Label>
            </td>
            <td>
                <asp:Label ID="LabelCustomerName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td >
                <asp:Label ID="LabelUploadFile" runat="server"></asp:Label>
            </td>
            <td></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="LabelFile" runat="server" Text="File"></asp:Label>
            </td>
            <td>    
                <div class="fileinputs">
                    <asp:FileUpload ID="FileUploadSelect" runat="server" CssClass="file" Onchange="return change();"/>
	                <div class="fakefile">
		                 <asp:TextBox ID="txtFilePath" runat="server" Width="210px"></asp:TextBox>
                         <asp:Button ID="ButtonBrowse" runat="server" Text="Browser" Width="80px"/>
                         <asp:Button ID="ButtonAddFile" Text ="Add" runat="server" OnClick="ButtonAdd_Click"  ValidationGroup="RegisterFile"/>                          
	                </div>
                </div>
            </td>     
        </tr>
    </table>
    </asp:Panel>
    
    <table>
        <tr>
         
            <td colspan="2">
                <asp:Label ID="LabelbUploadfilelist" runat="server" Text="Upload file list"></asp:Label>
            </td>                            
        </tr>
        <tr> 
            <td></td>            
            <td style="padding-left:10px">
                <asp:GridView ID="GridViewlistUpload" runat="server" AutoGenerateColumns="False" OnRowDataBound="GridViewlistUpload_RowDataBound" OnRowDeleting="GridViewlistUpload_RowDeleting" GridLines="None" ShowHeader="False">
                    <Columns>                
                        <asp:TemplateField>
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>.
                            </ItemTemplate>
                            <ItemStyle Width="10px" ></ItemStyle>
                        </asp:TemplateField>                        
                        <asp:BoundField HeaderText="ID" DataField="File_Sharing_ID">                 
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Name" DataField="File_Name" ItemStyle-Width="250px">                                                
                        </asp:BoundField>
                        <asp:TemplateField ItemStyle-Width="150px" ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Delete"  Text='<%# GetResource("ButtonLinkDelete")%>' ></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>                          
                    </Columns>
                </asp:GridView>
            </td>
        </tr>  
        </table>     

        <asp:ScriptManager
			ID="ScriptManager1"
			runat="server">
		</asp:ScriptManager>
          <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <table>
          <tr> 
          <td style="width:200px"></td>           
          <td style="width:40px">    
          <asp:UpdateProgress ID="UpdWaitImage" runat="server"  DynamicLayout="true" AssociatedUpdatePanelID="UpdatePanel1">
          <ProgressTemplate>
              <asp:Image ID="imgProgress" ImageUrl="~/Images/loading.gif" runat="server" />
          </ProgressTemplate>
         </asp:UpdateProgress>  
          </td>
        <td>
        <div style ="padding-top:10px">
          <asp:Button ID="ButtonUpload" runat="server" Text="Upload" OnClick="ButtonUpload_Click" />
          <asp:Button ID="ButtonClose" runat="server" Text="Close" OnClick="ButtonClose_Click"/>         
       </div>
        </td>
        </tr>
        </table>
         </ContentTemplate>
         </asp:UpdatePanel>        
         
        <asp:HiddenField ID="HiddenFieldID" runat="server" />
        <asp:HiddenField ID="HiddenFieldAccount" runat="server" />     
                  </td>
        </tr>
    </table>     
       <%--      </ContentTemplate>
               <Triggers>
                <asp:PostBackTrigger ControlID="ButtonAddFile"  />                                                
            </Triggers>
         </asp:UpdatePanel> --%>
</asp:Content>



