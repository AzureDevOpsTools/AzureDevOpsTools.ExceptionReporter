<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TFSExceptionUC.ascx.cs" Inherits="WebExceptionGUI.TFSExceptionUC" %>



<div style="margin-left:100px;margin-top:50px;">
    <div style="border: 2px ridge Black; padding: 5px; height: 25px; width: 850px; background-color:silver">
        <span style="font-size:22px; font-weight:bold">Feilrapportering:</span> Det oppstod en feil:
    </div>
    <div style="border: 2px ridge Black; padding: 5px; height: 80px; width: 850px;">
        <asp:TextBox ID="txtError" runat="server" Width="812px" TextMode="MultiLine" Rows="3" Columns="2" />
    </div>
    
    <div style="border: 2px ridge Black; padding: 5px; height: 26px; width: 850px;margin-top:10px ;background-color:silver">
        Beskriv gjerne stegene som førte frem til feilen. Og send inn til behandling.</div>
    <div style="border: 2px ridge Black; padding: 5px; height: 220px; width: 850px;">
        <asp:TextBox ID="txtDescrption" runat="server" TextMode="MultiLine" Rows="10" Columns="100" />
    </div>
    <div style="border: 2px ridge Black; padding: 5px; width: 850px; background-color:silver">
        <asp:Button ID="btnSend" runat="server" Text="Send" onclick="btnSend_Click" />&nbsp;&nbsp;
        <asp:Button ID="btnCancel" runat="server" Text="Avbryt" 
            onclick="btnCancel_Click1"  />
    </div>
    
    <p>
    <asp:label id="lblReportingErrorMsg" runat="server" />
    </p>
 
</div>