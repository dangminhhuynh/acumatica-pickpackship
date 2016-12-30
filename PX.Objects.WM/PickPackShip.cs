using System;
using System.Collections;
using PX.Data;
using PX.Objects.IN;
using System.Collections.Generic;
using PX.Objects.AR;
using PX.SM;

namespace PX.Objects.SO
{
    public static class ScanStatuses
    {
        public const string Success = "OK"; //Causes focus to be sent back to shipment nbr. field
        public const string Information = "INF";
        public const string Scan = "SCN";
        public const string Error = "ERR";
    }

    public static class ScanModes
    {
        public const string Add = "A";
        public const string Remove = "R";
    }

    public static class ScanCommands
    {
        public const char CommandChar = '*';

        public const string Clear = "Z";
        public const string Confirm = "C";
        public const string Add = "A";
        public const string Remove = "R";
        public const string Item = "I";
        public const string LotSerial = "S";
    }

    public class PickPackInfo : IBqlTable
    {
        public abstract class shipmentNbr : IBqlField { }
        [PXString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXDefault()]
        [PXUIField(DisplayName = "Shipment Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search2<SOShipment.shipmentNbr,
            InnerJoin<INSite, On<INSite.siteID, Equal<SOShipment.siteID>>,
            LeftJoinSingleTable<Customer, On<SOShipment.customerID, Equal<Customer.bAccountID>>>>,
            Where2<Match<INSite, Current<AccessInfo.userName>>,
            And<Where2<Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>,
            And<SOShipment.status, Equal<SOShipmentStatus.open>>>>>,
            OrderBy<Desc<SOShipment.shipmentNbr>>>))]
        public virtual string ShipmentNbr { get; set; }

        public abstract class barcode : IBqlField { }
        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Barcode")]
        public virtual string Barcode { get; set; }

        public abstract class quantity : IBqlField { }
        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "1.0")]
        [PXUIField(DisplayName = "Quantity")]
        public virtual decimal? Quantity { get; set; }

        public abstract class scanMode : IBqlField { }
        [PXString(1, IsFixed = true)]
        [PXStringList(new[] { ScanModes.Add, ScanModes.Remove }, new[] { PX.Objects.WM.Messages.Add, PX.Objects.WM.Messages.Remove })]
        [PXDefault(ScanModes.Add)]
        [PXUIField(DisplayName = "Scan Mode")]
        public virtual string ScanMode { get; set; }

        public abstract class lotSerialSearch : IBqlField { }
        [PXBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Search Lot/Serial Numbers", FieldClass = "LotSerial")]
        public virtual bool? LotSerialSearch { get; set; }

        public abstract class currentInventoryID : IBqlField { }
        [StockItem]
        public virtual int? CurrentInventoryID { get; set; } //User for lot/serial selection

        public abstract class currentSubID : IBqlField { }
        [SubItem]
        public virtual int? CurrentSubID { get; set; } //User for lot/serial selection

        public abstract class status : IBqlField { }
        [PXString(3, IsUnicode = true)]
        [PXUIField(DisplayName = "Status", Enabled = false, Visible = false)]
        public virtual string Status { get; set; }

        public abstract class message : IBqlField { }
        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Message", Enabled = false)]
        public virtual string Message { get; set; }
    }

    public class PickPackShip : PXGraph<PickPackShip>
    {
        public PXSetup<INSetup> Setup;
        public PXCancel<PickPackInfo> Cancel;
        public PXFilter<PickPackInfo> Document;
        public PXSelect<SOShipLinePick, Where<SOShipLinePick.shipmentNbr, Equal<Current<PickPackInfo.shipmentNbr>>>, OrderBy<Asc<SOShipLinePick.shipmentNbr, Asc<SOShipLine.lineNbr>>>> Transactions;
        public PXSelect<SOShipLineSplit, Where<SOShipLineSplit.shipmentNbr, Equal<Current<SOShipLinePick.shipmentNbr>>, And<SOShipLineSplit.lineNbr, Equal<Current<SOShipLinePick.lineNbr>>>>> Splits;

        protected void PickPackInfo_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            Transactions.Cache.AllowDelete = false;
            Transactions.Cache.AllowInsert = false;
            Splits.Cache.AllowDelete = false;
            Splits.Cache.AllowInsert = false;
            Splits.Cache.AllowUpdate = false;
        }

        protected void PickPackInfo_ShipmentNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var doc = e.Row as PickPackInfo;
            if (doc == null) return;

            doc.Status = ScanStatuses.Scan;
            doc.Message = String.Format("Shipment '{0}' loaded and ready to pick.", doc.ShipmentNbr);
            this.Document.Update(doc);
        }

        protected void SOShipLinePick_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
            PXUIFieldAttribute.SetEnabled<SOShipLinePick.pickedQty>(sender, e.Row, true);
        }
        
        protected IEnumerable splits()
        {
            //We only use this view as a container for picked lot/serial numbers. We don't care about what's in the DB for this shipment.
            foreach(var row in Splits.Cache.Cached)
            {
                yield return row;
            }
        }

        public PXAction<PickPackInfo> allocations;
        [PXUIField(DisplayName = "Allocations")]
        [PXButton]
        protected virtual void Allocations()
        {
            this.Splits.AskExt();
        }

        public PXAction<PickPackInfo> scan;
        [PXUIField(DisplayName = "Scan")]
        [PXButton]
        protected virtual void Scan()
        {
            var doc = this.Document.Current;

            if (String.IsNullOrEmpty(doc.Barcode))
            {
                doc.Status = ScanStatuses.Error;
                doc.Message = "Please scan a barcode.";
            }
            else
            {
                string barcode = String.Empty;
                if (doc.Barcode[0] == ScanCommands.CommandChar)
                {
                    var segments = doc.Barcode.Split(ScanCommands.CommandChar);
                    if (segments.Length > 3)
                    {
                        doc.Status = ScanStatuses.Error;
                        doc.Message = "Barcode contains too many commands and cannot be processed.";
                    }
                    else
                    {
                        ProcessCommand(segments[1]);
                        if (segments.Length > 2) ProcessBarcode(segments[2]);
                    }
                }
                else
                {
                    ProcessBarcode(doc.Barcode);
                }
            }

            doc.Barcode = String.Empty;
            this.Document.Update(doc);
        }

        protected virtual void ProcessCommand(string command)
        {
            var doc = this.Document.Current;

            int quantity = 0;
            if(int.TryParse(command, out quantity))
            {
                doc.Quantity = quantity;
                doc.Status = ScanStatuses.Information;
                doc.Message = String.Format("Quantity set to {0}.", quantity);
            }
            else
            {
                switch(command)
                {
                    case ScanCommands.Add:
                        this.Document.Current.ScanMode = ScanModes.Add;
                        doc.Status = ScanStatuses.Information;
                        doc.Message = "Add mode set."; //TODO: Think of a better message.
                        break;
                    case ScanCommands.Remove:
                        this.Document.Current.ScanMode = ScanModes.Remove;
                        doc.Status = ScanStatuses.Information;
                        doc.Message = "Remove mode set.";  //TODO: Think of a better message.
                        break;
                    case ScanCommands.Item:
                        this.Document.Current.LotSerialSearch = false;
                        doc.Status = ScanStatuses.Information;
                        doc.Message = "Ready to search by item barcode.";
                        break;
                    case ScanCommands.LotSerial:
                        this.Document.Current.LotSerialSearch = true;
                        doc.Status = ScanStatuses.Information;
                        doc.Message = "Ready to search by lot/serial number.";
                        break;
                    case ScanCommands.Confirm:
                        //Status/message will be set by Confirm action
                        this.Confirm.Press();
                        break;
                    case ScanCommands.Clear:
                        ClearScreen();
                        doc.Status = ScanStatuses.Success;
                        doc.Message = "Screen cleared.";
                        break;
                    default:
                        doc.Status = ScanStatuses.Error;
                        doc.Message = "Unknown command.";
                        break;
                }
            }
        }

        protected virtual void ClearScreen()
        {
            this.Document.Current.ShipmentNbr = null;
            this.Document.Current.CurrentInventoryID = null;
            this.Document.Current.CurrentSubID = null;
            this.Transactions.Cache.Clear();
            this.Splits.Cache.Clear();
        }

        protected virtual void ProcessBarcode(string barcode)
        {
            var doc = this.Document.Current;
            if (doc.LotSerialSearch == true)
            {
                //TODO: Implement lot/serial search mode
                throw new NotImplementedException();
            }

            if (doc.CurrentInventoryID == null)
            {
                ProcessItemBarcode(barcode);
            }
            else
            { 
                ProcessLotSerialBarcode(barcode);
            }
        }

        protected virtual void ProcessItemBarcode(string barcode)
        {
            var doc = this.Document.Current;

            var rec = (PXResult<INItemXRef, InventoryItem, INLotSerClass, INSubItem>)
                          PXSelectJoin<INItemXRef,
                            InnerJoin<InventoryItem,
                                            On<InventoryItem.inventoryID, Equal<INItemXRef.inventoryID>,
                                            And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.inactive>,
                                            And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.noPurchases>,
                                            And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.markedForDeletion>>>>>,
                            InnerJoin<INLotSerClass,
                                         On<InventoryItem.lotSerClassID, Equal<INLotSerClass.lotSerClassID>>,
                            InnerJoin<INSubItem,
                                         On<INSubItem.subItemID, Equal<INItemXRef.subItemID>>>>>,
                            Where<INItemXRef.alternateID, Equal<Required<PickPackInfo.barcode>>,
                                            And<INItemXRef.alternateType, Equal<INAlternateType.barcode>>>>
                            .SelectSingleBound(this, new object[] { doc }, barcode);

            if (rec != null)
            {
                var item = (InventoryItem)rec;
                var sub = (INSubItem)rec;
                var lsclass = (INLotSerClass)rec;

                if (lsclass.LotSerTrack != INLotSerTrack.NotNumbered &&
                    (lsclass.LotSerIssueMethod == INLotSerIssueMethod.UserEnterable ||
                    (lsclass.LotSerAssign == INLotSerAssign.WhenUsed && lsclass.AutoNextNbr == false)))
                {
                    if (lsclass.LotSerAssign == INLotSerAssign.WhenUsed && lsclass.LotSerTrackExpiration == true)
                    {
                        //TODO: Implement support for this.
                        throw new NotImplementedException("Lot/serial numbers that are assigned when used and which require tracking of expiration date are not supported with this tool.");
                    }

                    doc.CurrentInventoryID = item.InventoryID;
                    doc.CurrentSubID = sub.SubItemID;
                    doc.Status = ScanStatuses.Scan;
                    doc.Message = String.Format("Please scan lot/serial number for item {0}.", item.InventoryCD.TrimEnd());
                }
                else
                {
                    if (Document.Current.ScanMode == ScanModes.Add && AddPick(item.InventoryID, sub.SubItemID, Document.Current.Quantity, null))
                    {
                        doc.Status = ScanStatuses.Scan;
                        doc.Message = String.Format("Added {0} x {1}.", Document.Current.Quantity, item.InventoryCD.TrimEnd());
                        doc.Quantity = 1;
                    }
                    else if (Document.Current.ScanMode == ScanModes.Remove && RemovePick(item.InventoryID, sub.SubItemID, Document.Current.Quantity, null))
                    {
                        doc.Status = ScanStatuses.Scan;
                        doc.Message = String.Format("Removed {0} x {1}.", Document.Current.Quantity, item.InventoryCD.TrimEnd());
                        doc.Quantity = 1;
                        doc.ScanMode = ScanModes.Add;
                    }
                    else
                    {
                        doc.Status = ScanStatuses.Error;
                        doc.Message = String.Format("Item {0} not found on shipment.", item.InventoryCD.TrimEnd());
                    }
                }
            }
            else
            {
                doc.Status = ScanStatuses.Error;
                doc.Message = String.Format("Barcode '{0}' not found in database.", barcode);
            }
        }

        private void ProcessLotSerialBarcode(string barcode)
        {
            var doc = this.Document.Current;
            var inventoryItem = (InventoryItem)PXSelectorAttribute.Select<PickPackInfo.currentInventoryID>(Document.Cache, Document.Current);

            if (Document.Current.ScanMode == ScanModes.Add)
            {
                if (AddPick(doc.CurrentInventoryID, doc.CurrentSubID, Document.Current.Quantity, barcode))
                {
                    doc.Status = ScanStatuses.Scan;
                    doc.Message = String.Format("Added {0} x {1} ({2}).", Document.Current.Quantity, inventoryItem.InventoryCD.TrimEnd(), barcode);
                    doc.Quantity = 1;
                }
                else
                {
                    doc.Status = ScanStatuses.Error;
                    doc.Message = String.Format("Item {0} not found on shipment.", inventoryItem.InventoryCD.TrimEnd());
                }
            }
            else if (Document.Current.ScanMode == ScanModes.Remove)
            {
                if (RemovePick(doc.CurrentInventoryID, doc.CurrentSubID, Document.Current.Quantity, barcode))
                {
                    doc.Status = ScanStatuses.Scan;
                    doc.Message = String.Format("Removed {0} x {1} ({2}).", Document.Current.Quantity, inventoryItem.InventoryCD.TrimEnd(), barcode);
                    doc.Quantity = 1;
                    doc.ScanMode = ScanModes.Add;
                }
                else
                {
                    doc.Status = ScanStatuses.Error;
                    doc.Message = String.Format("Lot/serial {0} not found in sufficient quantity on shipment.", barcode);
                }
            }

            doc.CurrentInventoryID = null;
            doc.CurrentSubID = null;
        }

        protected virtual bool AddPick(int? inventoryID, int? subID, decimal? quantity, string lotSerialNumber)
        {
            SOShipLinePick firstLine = null;
            foreach (SOShipLinePick pickLine in this.Transactions.Select())
            {
                if (pickLine.InventoryID == inventoryID && (pickLine.SubItemID == subID || Setup.Current.UseInventorySubItem == false))
                {
                    if (firstLine == null) firstLine = pickLine;

                    if (pickLine.PickedQty.GetValueOrDefault() < pickLine.ShippedQty.GetValueOrDefault())
                    {
                        //We first try to fill all the lines sequentially - item may be present multiple times on the shipment
                        decimal quantityForCurrentPickLine = Math.Min(quantity.GetValueOrDefault(), pickLine.ShippedQty.GetValueOrDefault() - pickLine.PickedQty.GetValueOrDefault());
                        pickLine.PickedQty = pickLine.PickedQty.GetValueOrDefault() + quantityForCurrentPickLine;
                        this.Transactions.Update(pickLine);
                        if (lotSerialNumber != null) AddLotSerialToCurrentLineSplits(lotSerialNumber, quantityForCurrentPickLine);
                        quantity = quantity - quantityForCurrentPickLine;
                    }

                    if(quantity == 0)
                    {
                        return true;
                    }
                }
            }

            if (firstLine != null)
            {
                //All the lines are already filled; just over-pick the first one.
                firstLine.PickedQty = firstLine.PickedQty.GetValueOrDefault() + quantity;
                this.Transactions.Update(firstLine);
                if (lotSerialNumber != null) AddLotSerialToCurrentLineSplits(lotSerialNumber, quantity.GetValueOrDefault());
                return true;
            }
            else
            {
                //Item not found.
                return false;
            }
        }

        protected virtual void AddLotSerialToCurrentLineSplits(string lotSerial, decimal quantity)
        {
            //TODO: See if it's possible to set quantity > 1 in some cases with lot/numered (shipment 000201 didn't work out in this way, hence the loop)
            for (int i = 0; i < quantity; i++)
            {
                var split = (SOShipLineSplit)this.Splits.Cache.CreateInstance();
                split.Qty = 1;
                split.LocationID = this.Transactions.Current.LocationID;
                split.LotSerialNbr = lotSerial;
                this.Splits.Insert(split);
            }
        }

        protected virtual decimal GetTotalQuantityPicked(int? inventoryID, int? subID, string lotSerialNumber)
        {
            decimal total = 0;

            foreach (SOShipLinePick pickLine in this.Transactions.Select())
            {
                if(pickLine.InventoryID == inventoryID && pickLine.SubItemID == subID)
                {
                    this.Transactions.Current = pickLine;
                    foreach(SOShipLineSplit split in this.Splits.Select())
                    {
                        if(split.LotSerialNbr == lotSerialNumber)
                        {
                            total = total + split.Qty.GetValueOrDefault();
                        }
                    }
                }
            }

            return total;
        }

        protected virtual bool RemovePick(int? inventoryID, int? subID, decimal? quantity, string lotSerialNumber)
        {
            if(lotSerialNumber != null)
            {
                //First validate that we are fully able to remove the requested quantity for this lot/serial number
                if(GetTotalQuantityPicked(inventoryID, subID, lotSerialNumber) < quantity)
                {
                    // We stop there otherwise we'll end up with a partial removal of this pick
                    return false;
                }
            }

            SOShipLinePick firstLine = null;
            foreach (SOShipLinePick pickLine in this.Transactions.Select())
            {
                if (pickLine.InventoryID == inventoryID && (pickLine.SubItemID == subID || Setup.Current.UseInventorySubItem == false))
                {
                    if (firstLine == null) firstLine = pickLine;

                    if (pickLine.PickedQty.GetValueOrDefault() > 0)
                    {
                        //We first try to remove the quantities from already scanned quantities sequentially - item may be present multiple times on the shipment
                        decimal quantityForCurrentPickLine = Math.Min(quantity.GetValueOrDefault(), pickLine.PickedQty.GetValueOrDefault());
                        pickLine.PickedQty = pickLine.PickedQty.GetValueOrDefault() - quantityForCurrentPickLine;
                        if (pickLine.PickedQty == 0) pickLine.PickedQty = null;

                        this.Transactions.Update(pickLine);
                        if (lotSerialNumber != null) RemoveLotSerialFromCurrentLineSplits(lotSerialNumber, quantityForCurrentPickLine);
                        quantity = quantity - quantityForCurrentPickLine;
                    }

                    if (quantity == 0)
                    {
                        return true;
                    }
                }
            }

            if (firstLine != null)
            {
                //All the lines are already cleared; deduct from the first one which will go negative (this will be validated during Confirm step).
                firstLine.PickedQty = firstLine.PickedQty.GetValueOrDefault() - quantity;
                if (firstLine.PickedQty == 0) firstLine.PickedQty = null;
                this.Transactions.Update(firstLine);
                if (lotSerialNumber != null) RemoveLotSerialFromCurrentLineSplits(lotSerialNumber, quantity.GetValueOrDefault());
                return true;
            }
            else
            {
                //Item not found.
                return false;
            }
        }

        protected virtual void RemoveLotSerialFromCurrentLineSplits(string lotSerial, decimal quantity)
        {
            foreach(SOShipLineSplit split in this.Splits.Select())
            {
                if(split.LotSerialNbr == lotSerial)
                {
                    if (split.Qty != 1)
                    {
                        throw new PXException("Unexpected split quantity for lot/serial {0} (Quantity: {1}).", lotSerial, split.Qty);
                    }

                    this.Splits.Delete(split);
                    quantity = quantity - 1;
                }

                if (quantity == 0) break;
            }
            
            if(quantity != 0)
            {
                // This condition is validated in RemovePick, so we should never get to this point.
                throw new PXException("The system was not able to locate lot/serial number {0} (Quantity: {1}). Please check the shipment.", lotSerial, quantity);
            }
        }

        public PXAction<PickPackInfo> Confirm;
        [PXUIField(DisplayName = "Confirm")]
        [PXButton]
        protected virtual void confirm()
        {
            var doc = this.Document.Current;
            doc.Status = ScanStatuses.Information;
            doc.Message = String.Empty;
            SOShipmentEntry graph = PXGraph.CreateInstance<SOShipmentEntry>();
            SOShipment shipment = null;

            if (doc.ShipmentNbr != null)
            {
                shipment = graph.Document.Search<SOShipment.shipmentNbr>(doc.ShipmentNbr);
            }

            if (shipment == null)
            {
                doc.Status = ScanStatuses.Error;
                doc.Message = "Shipment not found.";
                this.Document.Update(doc);
            }
            else
            {
                if (!IsConfirmationNeeded() ||
                    this.Document.Ask("The quantity picked for one or more lines doesn't match with the shipment. Do you want to continue?", MessageButtons.YesNo) == PX.Data.WebDialogResult.Yes)
                {
                    PXLongOperation.StartOperation(this, () =>
                    {
                        try
                        {
                            graph.Document.Current = shipment;
                            UpdateShipmentWithPickResults(graph);

                            PXAction confAction = graph.Actions["Action"];
                            var adapter = new PXAdapter(new DummyView(graph, graph.Document.View.BqlSelect, new List<object> { graph.Document.Current }));
                            adapter.Menu = SOShipmentEntryActionsAttribute.Messages.ConfirmShipment;
                            confAction.PressButton(adapter);

                            PreparePrintJobs(graph);

                            ClearScreen();

                            doc.Status = ScanStatuses.Success;
                            doc.Message = String.Format("Shipment {0} confirmed.", doc.ShipmentNbr);
                            
                            this.Document.Update(doc);
                        }
                        catch (Exception e)
                        {
                            doc.Status = ScanStatuses.Error;
                            doc.Message = e.Message;
                            this.Document.Update(doc);
                            throw;
                        }
                    });
                }
            }
        }
        
        protected virtual void PreparePrintJobs(SOShipmentEntry graph)
        {
            //TODO: Add options to decide what should be printed and to which queue.
            var jobMaint = PXGraph.CreateInstance<PX.SM.PrintJobMaint>();

            //Shipment confirmation
            AddPrintJob(jobMaint, "GAB1", "SO642000", new Dictionary<string, string> { { "ShipmentNbr", graph.Document.Current.ShipmentNbr } });

            //Shipment labels
            UploadFileMaintenance ufm = PXGraph.CreateInstance<UploadFileMaintenance>();
            foreach (SOPackageDetail package in graph.Packages.Select())
            {
                Guid[] files = PXNoteAttribute.GetFileNotes(graph.Packages.Cache, package);
                foreach (Guid id in files)
                {
                    FileInfo fileInfo = ufm.GetFile(id);
                    string extension = System.IO.Path.GetExtension(fileInfo.Name).ToLower();
                    if(extension == ".pdf")
                    {
                        AddPrintJob(jobMaint, "GAB1", "", new Dictionary<string, string> { { "FILEID", id.ToString() } });
                    }
                    else
                    {
                        //TODO: Add support for other file types - ZPL, EPL, etc...
                        PXTrace.WriteWarning("Unsupported file extension attached to the package for Shipment {0}/{1}", graph.Document.Current.ShipmentNbr, package.LineNbr);
                    }
                }
            }
        }

        protected virtual void AddPrintJob(PX.SM.PrintJobMaint graph, string printQueue, string reportID, Dictionary<string, string> parameters)
        {
            var job = (PX.SM.SMPrintJob)graph.Job.Cache.CreateInstance();
            job.PrintQueue = printQueue;
            job.ReportID = reportID;
            graph.Job.Insert(job);

            foreach (var p in parameters)
            {
                var parameter = (PX.SM.SMPrintJobParameter) graph.Parameters.Cache.CreateInstance();
                parameter.ParameterName = p.Key;
                parameter.ParameterValue = p.Value;
                graph.Parameters.Insert(parameter);
            }

            graph.Actions.PressSave();
        }
        protected virtual bool IsConfirmationNeeded()
        {
            foreach (SOShipLinePick pickLine in this.Transactions.Select())
            {
                if (pickLine.PickedQty.GetValueOrDefault() != pickLine.ShippedQty.GetValueOrDefault())
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual void UpdateShipmentWithPickResults(SOShipmentEntry graph)
        {
            foreach(SOShipLinePick pickLine in this.Transactions.Select())
            {
                graph.Transactions.Current = graph.Transactions.Search<SOShipLine.lineNbr>(pickLine.LineNbr);
                if(graph.Transactions.Current != null)
                {
                    //Update shipped quantity to match what was picked
                    graph.Transactions.Current.ShippedQty = pickLine.PickedQty.GetValueOrDefault();
                    graph.Transactions.Update(graph.Transactions.Current);

                    //Set any lot/serial numbers that were assigned
                    bool initialized = false;
                    foreach(SOShipLineSplit split in this.Splits.Select())
                    {
                        if(this.Splits.Cache.GetStatus(split) == PXEntryStatus.Inserted)
                        {
                            if(!initialized)
                            {
                                //Delete any pre-existing split
                                foreach(SOShipLineSplit s in graph.splits.Select())
                                {
                                    graph.splits.Delete(s);
                                }
                                initialized = true;
                            }

                            graph.splits.Insert(split);
                        }
                    }
                }
                else
                {
                    throw new PXException("Line {0} not found in shipment.", pickLine.LineNbr);
                }
            }

            foreach(SOPackageDetail package in graph.Packages.Select())
            {
                //TODO: Add proper support for packages
                package.Confirmed = true;
                graph.Packages.Update(package);
            }
        }

        private sealed class DummyView : PXView
        {
            private readonly List<object> _records;
            internal DummyView(PXGraph graph, BqlCommand command, List<object> records)
                : base(graph, true, command)
            {
                _records = records;
            }
            public override List<object> Select(object[] currents, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
            {
                return _records;
            }
        }
    }
}
