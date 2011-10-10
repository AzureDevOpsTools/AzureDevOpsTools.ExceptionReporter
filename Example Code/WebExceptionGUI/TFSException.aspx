<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/ExceptionDefault.Master" CodeBehind="TFSException.aspx.cs" Inherits="WebExceptionGUI.TFSException" %>



<%@ Register src="TFSExceptionUC.ascx" tagname="TFSExceptionUC" tagprefix="uc1" %>



<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >
    

    
    <uc1:TFSExceptionUC ID="TFSExceptionUC1" runat="server" />
    

    
</asp:Content>
