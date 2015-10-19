<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Site.Master" AutoEventWireup="true" CodeBehind="Download.aspx.cs" Inherits="FTSS.Web.Form.Download" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Button ID="cmdCreateContainer" runat="server" OnClick="cmdCreateContainer_Click" Text="Create Container" />
    <br />
    <br />
    <asp:FileUpload ID="FileUpload1" runat="server" />
    <br />
    <br />
    <asp:Button ID="cmdUpload" runat="server" OnClick="cmdUpload_Click" Text="Upload" />
    <asp:Button ID="cmdListFile" runat="server" OnClick="cmdListFile_Click" Text="Show File" />
    <asp:Button ID="cmdDownload" runat="server" OnClick="cmdDownload_Click" Text="Download" />
    <asp:Button ID="cmdDeleteFile" runat="server" OnClick="cmdDeleteFile_Click" Text="Delete" />
    <br />
    <br />
    <br />
    <asp:ListBox ID="ListBox1" runat="server" Height="176px" Width="662px"></asp:ListBox>
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
</asp:Content>
