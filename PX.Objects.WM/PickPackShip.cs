using System;
using System.Collections;
using PX.Data;
using PX.Objects.IN;
using System.Collections.Generic;
using PX.Objects.AR;
using PX.SM;

namespace PX.Objects.SO
{
    public class PickPackStatus
    {
        public const string Success = "OK";
        public const string Information = "INF";
        public const string Scan = "SCN";
        public const string Error = "ERR";
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

        public abstract class currentInventoryID : IBqlField { }
        [PXInt]
        [PXUIField(DisplayName = "Current Item (for lot/serial selection)")]
        public virtual int? CurrentInventoryID { get; set; }

        public abstract class currentSubID : IBqlField { }
        [PXInt]
        [PXUIField(DisplayName = "Current Sub (for lot/serial selection)")]
        public virtual int? CurrentSubID { get; set; }

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
        }

        protected void PickPackInfo_ShipmentNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var doc = e.Row as PickPackInfo;
            if (doc == null) return;

            doc.Status = PickPackStatus.Scan;
            doc.Message = String.Format("Shipment '{0}' loaded and ready to pick.", doc.ShipmentNbr);
            this.Document.Update(doc);
        }

        protected void SOShipLinePick_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
            PXUIFieldAttribute.SetEnabled<SOShipLinePick.pickedQty>(sender, e.Row, true);
        }

        public PXAction<PickPackInfo> scan;
        [PXUIField(DisplayName = "Scan")]
        [PXButton]
        protected virtual void Scan()
        {
            var doc = this.Document.Current;

            if (String.IsNullOrEmpty(doc.Barcode))
            {
                doc.Status = PickPackStatus.Error;
                doc.Message = "Please scan a barcode.";
            }
            else
            {
                if (doc.CurrentInventoryID == null)
                {
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
                                Where<INItemXRef.alternateID, Equal<Current<PickPackInfo.barcode>>,
                                                And<INItemXRef.alternateType, Equal<INAlternateType.barcode>>>>
                                .SelectSingleBound(this, new object[] { doc });

                    if (rec != null)
                    {
                        var item = (InventoryItem)rec;
                        var sub = (INSubItem)rec;
                        var lsclass = (INLotSerClass)rec;

                        if (lsclass.LotSerTrack != INLotSerTrack.NotNumbered && 
                            (lsclass.LotSerIssueMethod == INLotSerIssueMethod.UserEnterable || 
                            (lsclass.LotSerAssign == INLotSerAssign.WhenUsed && lsclass.AutoNextNbr == false)))
                        {
                            if(lsclass.LotSerAssign == INLotSerAssign.WhenUsed && lsclass.LotSerTrackExpiration == true)
                            {
                                //TODO: Implement support for this.
                                throw new NotImplementedException("Lot/serial numbers that are assigned when used and which require tracking of expiration date are not supported with this tool.");
                            }

                            doc.CurrentInventoryID = item.InventoryID;
                            doc.CurrentSubID = sub.SubItemID;
                            doc.Status = PickPackStatus.Scan;
                            doc.Message = String.Format("Please scan lot/serial number for item {0}.", item.InventoryCD.TrimEnd());
                        }
                        else
                        {
                            if (AddPick(item.InventoryID, sub.SubItemID, Document.Current.Quantity, null))
                            {
                                doc.Status = PickPackStatus.Scan;
                                doc.Message = String.Format("Item '{0}' was picked. Ready to pick next item.", item.InventoryCD.TrimEnd());
                                doc.Quantity = 1;
                            }
                            else
                            {
                                doc.Status = PickPackStatus.Error;
                                doc.Message = String.Format("Item '{0}' not found on shipment.", item.InventoryCD.TrimEnd());
                            }
                        }
                    }
                    else
                    {
                        doc.Status = PickPackStatus.Error;
                        doc.Message = String.Format("Barcode '{0}' not found in database.", doc.Barcode);
                    }
                }
                else
                {
                    if (AddPick(doc.CurrentInventoryID, doc.CurrentSubID, Document.Current.Quantity, doc.Barcode))
                    {
                        doc.Status = PickPackStatus.Scan;
                        doc.Message = String.Format("Lot/serial '{0}' was picked. Ready to pick next item.", doc.Barcode);
                        doc.Quantity = 1;
                    }
                    else
                    {
                        doc.Status = PickPackStatus.Error;
                        doc.Message = String.Format("Item not found on shipment.");
                    }

                    doc.CurrentInventoryID = null;
                    doc.CurrentSubID = null;
                }
            }

            doc.Barcode = String.Empty;
            this.Document.Update(doc);
        }

        protected virtual bool AddPick(int? inventoryID, int? subID, decimal? quantity, string lotSerialNumber)
        {
            //TODO: Needs to be updated to properly handle case where quantity is > 1 and more than one line exist for item - we can't assign everything to first line but need to distribute.
            SOShipLinePick firstLine = null;
            foreach (SOShipLinePick pickLine in this.Transactions.Select())
            {
                if (pickLine.InventoryID == inventoryID && (pickLine.SubItemID == subID || Setup.Current.UseInventorySubItem == false))
                {
                    if (firstLine == null) firstLine = pickLine;

                    //If the item is present multiple times on the shipment, we first try to fill all the lines sequentially
                    if (pickLine.PickedQty.GetValueOrDefault() + quantity.GetValueOrDefault() <= pickLine.ShippedQty.GetValueOrDefault())
                    {
                        pickLine.PickedQty = pickLine.PickedQty.GetValueOrDefault() + quantity;
                        this.Transactions.Update(pickLine);
                        if (lotSerialNumber != null) AddLotSerialToCurrentLineSplits(lotSerialNumber, quantity.GetValueOrDefault());
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

        public PXAction<PickPackInfo> confirm;
        [PXUIField(DisplayName = "Confirm")]
        [PXButton]
        protected virtual void Confirm()
        {
            var doc = this.Document.Current;
            doc.Status = PickPackStatus.Information;
            doc.Message = String.Empty;

            SOShipmentEntry graph = PXGraph.CreateInstance<SOShipmentEntry>();
            SOShipment shipment = null;

            if (doc.ShipmentNbr != null)
            {
                shipment = graph.Document.Search<SOShipment.shipmentNbr>(doc.ShipmentNbr);
            }

            if (shipment == null)
            {
                doc.Status = PickPackStatus.Error;
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

                            doc.Status = PickPackStatus.Success;
                            doc.Message = String.Format("Shipment {0} confirmed.", doc.ShipmentNbr);
                            doc.ShipmentNbr = null;
                            this.Document.Update(doc);

                            this.Transactions.Cache.Clear();
                            this.Splits.Cache.Clear();
                        }
                        catch (Exception e)
                        {
                            doc.Status = PickPackStatus.Error;
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
