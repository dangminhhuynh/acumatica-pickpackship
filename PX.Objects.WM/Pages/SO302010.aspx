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
        <AutoSize Container="Window" Enabled="True" MinHeight="400" />
    </px:PXGrid>
</asp:content>
