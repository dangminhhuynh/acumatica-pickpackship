<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO302010.aspx.cs"
    Inherits="Page_SO302010" Title="Pick, Pack and Ship" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:content id="cont1" contentplaceholderid="phDS" runat="Server">
	<script language="javascript" type="text/javascript">
	    function CommandResult(ds, context) {
	        var baseUrl = (location.href.indexOf("HideScript") > 0) ? "../../" : "../../../";
	        var edStatus = px_alls["edStatus"];

	        if ((context.command == "Confirm" || context.command == "Scan") && edStatus != null)
	        {
	            if (edStatus.getValue() == "OK")
                {
	                var audio = new Audio(baseUrl + 'Sounds/success.wav');
	                audio.play();

	                px_alls["edShipmentNbr"].focus();
	            }
	            else if (edStatus.getValue() == "SCN")
	            {
	                var audio = new Audio(baseUrl + 'Sounds/balloon.wav');
	                audio.play();
	            }
	            else if(edStatus.getValue() == "ERR")
	            {
	                var audio = new Audio(baseUrl + 'Sounds/asterisk.wav');
	                audio.play();
	            }
	        }
	    }

	    function Barcode_KeyDown(ctrl, e) {
	        if (e.keyCode === 13) { //Enter key
	            var ds = px_alls["ds"];
	            ds.executeCallback("Scan");
	            e.cancel = true;
	        }
	    }
	</script>

	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.SO.PickPackShip" PrimaryView="Document">
        <ClientEvents CommandPerformed="CommandResult"/>
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Confirm" Visible="True" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="Scan" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="Allocations" Visible="False" CommitChanges="true" DependOnGrid="grid" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:content>
<asp:content id="cont2" contentplaceholderid="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Height="63px" Width="100%" Visible="true" DataMember="Document" DefaultControlID="edShipmentNbr">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="L" />
			<px:PXSelector ID="edShipmentNbr" runat="server" DataField="ShipmentNbr" AutoRefresh="true" AllowEdit="true" CommitChanges="true" />
            <px:PXTextEdit ID="edBarcode" runat="server" DataField="Barcode">
                <ClientEvents KeyDown="Barcode_KeyDown" />
            </px:PXTextEdit>
            
            <px:PXCheckBox ID="edLotSerialSearch" runat="server" DataField="LotSerialSearch" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="L" ColumnWidth="M" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edMessage" runat="server" DataField="Message" SuppressLabel="true" />
            <px:PXNumberEdit ID="edQuantity" runat="server" DataField="Quantity" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXGroupBox ID="gbMode" runat="server" Caption="Scan Mode" DataField="ScanMode" RenderSimple="True" RenderStyle="Simple">
                <Template>
                    <px:PXRadioButton ID="rbAdd" runat="server" GroupName="gbMode"
                        Text="Add" Value="A" />
                    <px:PXRadioButton ID="rbRemove" runat="server" GroupName="gbMode"
                        Text="Remove" Value="R" />
                </Template>
                <ContentLayout Layout="Stack" Orientation="Horizontal" />
            </px:PXGroupBox>
           
            <%--Always hidden, used by JavaScript to decide which sound to play--%>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" />
            <px:PXTextEdit ID="edStatus" runat="server" DataField="Status" SuppressLabel="true" />
        </Template>
    </px:PXFormView>
</asp:content>
<asp:content id="cont3" contentplaceholderid="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="height: 250px;" Width="100%" 
        OnRowDataBound="grid_RowDataBound" SkinID="Inquire" StatusField="Availability" SyncPosition="true" Height="372px" TabIndex="-7372">
        <Levels>
            <px:PXGridLevel DataMember="Transactions" DataKeyNames="ShipmentNbr,LineNbr">
                <Columns>
                    <px:PXGridColumn DataField="Availability" Width="1px" />
                    <px:PXGridColumn DataField="ShipmentNbr" Width="90px" />
                    <px:PXGridColumn DataField="LineNbr" TextAlign="Right" Width="54px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="OrigOrderType" Width="36px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="OrigOrderNbr" Width="90px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="OrigLineNbr" TextAlign="Right" Width="54px" />
                    <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" AutoCallBack="True" RenderEditorText="True" />
                    <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A" Width="45px" NullText="<SPLIT>" AutoCallBack="True" />
                    <px:PXGridColumn AllowNull="False" DataField="IsFree" TextAlign="Center" Type="CheckBox" />
                    <px:PXGridColumn DataField="SiteID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" AutoCallBack="True" />
                    <px:PXGridColumn DataField="LocationID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" NullText="<SPLIT>" />
                    <px:PXGridColumn DataField="UOM" Width="54px" AutoCallBack="True" />
                    <px:PXGridColumn AllowNull="False" DataField="PickedQty" TextAlign="Right" Width="81px" />
                    <px:PXGridColumn AllowNull="False" AutoCallBack="True" DataField="ShippedQty" TextAlign="Right" Width="81px" />
                    <px:PXGridColumn AllowNull="False" DataField="OriginalShippedQty" TextAlign="Right" Width="81px" />
                    <px:PXGridColumn AllowNull="False" DataField="OrigOrderQty" TextAlign="Right" Width="81px" />
                    <px:PXGridColumn AllowNull="False" DataField="OpenOrderQty" TextAlign="Right" Width="81px" />
                    <px:PXGridColumn AllowNull="False" DataField="CompleteQtyMin" TextAlign="Right" Width="81px" />
                    <px:PXGridColumn DataField="LotSerialNbr" Width="180px" NullText="<SPLIT>" />
                    <px:PXGridColumn DataField="ShipComplete" Width="117px" RenderEditorText="True" />
                    <px:PXGridColumn DataField="ExpireDate" Width="90px" />
                    <px:PXGridColumn DataField="ReasonCode" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" />
                    <px:PXGridColumn DataField="TranDesc" Width="180px" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <ActionBar>
            <CustomItems>
                <px:PXToolBarButton Text="Allocations" Key="cmdLS" CommandName="Allocations" CommandSourceID="ds" DependOnGrid="grid" />
            </CustomItems>
        </ActionBar>
        <AutoSize Container="Window" Enabled="True" MinHeight="400" />
    </px:PXGrid>
    <%-- Settings --%>
    <px:PXSmartPanel ID="PanelPrintSettings" runat="server" Height="150px" Width="400px" Caption="Settings" CaptionVisible="True"
        Key="PrintSetup" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True" AutoCallBack-Target="frmPrintSettings">
        <px:PXFormView ID="frmPrintSettings" runat="server" DataSourceID="ds" DataMember="PrintSetup" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule1" runat="server" LabelsWidth="M" ControlSize="M" StartGroup="True" SuppressLabel="True" GroupCaption="Shipment Confirmation"/>
                <px:PXCheckBox ID="edShipmentConfirmation" runat="server" DataField="ShipmentConfirmation" CommitChanges="true" />
                <px:PXLayoutRule ID="PXLayoutRule3" runat="server" LabelsWidth="M" ControlSize="M" SuppressLabel="False"/>
                <px:PXSelector ID="edShipmentConfirmationQueue" runat="server" DataField="ShipmentConfirmationQueue" CommitChanges="true" />
                
                <px:PXLayoutRule ID="PXLayoutRule2" runat="server" LabelsWidth="M" ControlSize="M" StartGroup="True" SuppressLabel="True" GroupCaption="Shipment Labels"/>
                <px:PXCheckBox ID="edShipmentLabels" runat="server" DataField="ShipmentLabels" CommitChanges="true" />
                <px:PXLayoutRule ID="PXLayoutRule5" runat="server" LabelsWidth="M" ControlSize="M" SuppressLabel="False"/>
                <px:PXSelector ID="edShipmentLabelsQueue" runat="server" DataField="ShipmentLabelsQueue" CommitChanges="true" />
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
            <px:PXButton ID="pbClose" runat="server" DialogResult="OK" Text="Close"/>
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Bin/Lot/Serial Numbers --%>
    <px:PXSmartPanel ID="PanelLS" runat="server" Width="764px" Caption="Allocations" CaptionVisible="True"
        Key="Splits" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True" AutoCallBack-Target="grid2">
        <px:PXGrid ID="grid2" runat="server" Width="100%" AutoAdjustColumns="True" DataSourceID="ds" Style="border-width: 1px 0px; left: 0px; top: 0px; height: 192px;" SyncPosition="true">
            <AutoSize Enabled="true" />
            <Mode InitNewRow="True" />
            <Parameters>
                <px:PXSyncGridParam ControlID="grid" />
            </Parameters>
            <Levels>
                <px:PXGridLevel DataMember="splits">
                    <Columns>
                        <px:PXGridColumn DataField="InventoryID" Width="108px" />
                        <px:PXGridColumn DataField="SubItemID" Width="108px" />
                        <px:PXGridColumn DataField="LocationID" AllowShowHide="Server" Width="108px" CommitChanges="true" />
                        <px:PXGridColumn DataField="LotSerialNbr" AllowShowHide="Server" Width="108px" CommitChanges="true" />
                        <px:PXGridColumn DataField="Qty" Width="108px" TextAlign="Right" />
                        <px:PXGridColumn DataField="UOM" Width="108px" />
                        <px:PXGridColumn DataField="ExpireDate" AllowShowHide="Server" Width="90px" />
                        <px:PXGridColumn AllowUpdate="False" DataField="InventoryID_InventoryItem_descr" Width="108px" />
                    </Columns>
                    <RowTemplate>
                        <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                        <px:PXSegmentMask ID="edSubItemID2" runat="server" DataField="SubItemID" AutoRefresh="true" />
                        <px:PXSegmentMask ID="edLocationID2" runat="server" DataField="LocationID" AutoRefresh="true">
                            <Parameters>
                                <px:PXControlParam ControlID="grid2" Name="SOShipLineSplit.siteID" PropertyName="DataValues[&quot;SiteID&quot;]" Type="String" />
                                <px:PXControlParam ControlID="grid2" Name="SOShipLineSplit.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                <px:PXControlParam ControlID="grid2" Name="SOShipLineSplit.subItemID" PropertyName="DataValues[&quot;SubItemID&quot;]" Type="String" />
                            </Parameters>
                        </px:PXSegmentMask>
                        <px:PXNumberEdit ID="edQty2" runat="server" DataField="Qty" />
                        <px:PXSelector ID="edUOM2" runat="server" DataField="UOM" AutoRefresh="true">
                            <Parameters>
                                <px:PXControlParam ControlID="grid" Name="SOShipLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                            </Parameters>
                        </px:PXSelector>
                        <px:PXSelector ID="edLotSerialNbr2" runat="server" DataField="LotSerialNbr" AutoRefresh="true">
                            <Parameters>
                                <px:PXControlParam ControlID="grid2" Name="SOShipLineSplit.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                <px:PXControlParam ControlID="grid2" Name="SOShipLineSplit.subItemID" PropertyName="DataValues[&quot;SubItemID&quot;]" Type="String" />
                                <px:PXControlParam ControlID="grid2" Name="SOShipLineSplit.locationID" PropertyName="DataValues[&quot;LocationID&quot;]" Type="String" />
                            </Parameters>
                        </px:PXSelector>
                        <px:PXDateTimeEdit ID="edExpireDate2" runat="server" DataField="ExpireDate" />
                    </RowTemplate>
                    <Layout ColumnsMenu="False" />
                </px:PXGridLevel>
            </Levels>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnSave" runat="server" DialogResult="OK" Text="OK" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:content>
