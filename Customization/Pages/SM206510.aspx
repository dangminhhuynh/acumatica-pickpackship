<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SM206510.aspx.cs" Inherits="Page_SM206510" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:pxdatasource id="ds" width="100%" runat="server" typename="PX.SM.PrintQueueMaint" primaryview="Queues" Visible="True">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Save" CommitChanges="True"/>
        </CallbackCommands>
    </px:pxdatasource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
    <px:PXGrid id="grid" runat="server" height="400px" width="100%" allowpaging="True" adjustpagesize="Auto" allowsearch="True" skinid="Primary">
        <Levels>
            <px:PXGridLevel DataMember="Queues">
                <Columns>
                    <px:PXGridColumn DataField="PrintQueue" Width="100px"/>
                    <px:PXGridColumn DataField="Descr" Width="300px"/>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="400" ></AutoSize>
    </px:PXGrid>
</asp:Content>