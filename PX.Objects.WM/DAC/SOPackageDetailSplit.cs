using PX.Data;
using PX.Objects.IN;
using PX.Objects.SO;
using System;

namespace PX.SM
{
    [Serializable]
    public class SOPackageDetailSplit : IBqlTable
    {
        public abstract class shipmentNbr : IBqlField { }

        [PXDBString(15, IsKey = true, IsUnicode = true)]
        [PXDBDefault(typeof(SOPackageDetail.shipmentNbr))]
        [PXParent(typeof(Select<SOPackageDetail,
                         Where<SOPackageDetail.shipmentNbr, Equal<Current<shipmentNbr>>,
                         And<SOPackageDetail.lineNbr, Equal<Current<lineNbr>>>>>))]
        public virtual String ShipmentNbr { get; set; }
        
        public abstract class lineNbr : IBqlField { }

        [PXDBInt(IsKey = true)]
        [PXDefault(typeof(SOPackageDetail.lineNbr))]
        public virtual Int32? LineNbr { get; set; }

        public abstract class splitLineNbr : IBqlField { }
        
        [PXDBInt(IsKey = true)]
        [PXDefault]
        [PXLineNbr(typeof(SOShipment.lineCntr))]
        public virtual Int32? SplitLineNbr { get; set; }

        public abstract class inventoryID : IBqlField { }

        [PXDefault]
        [Inventory]
        public virtual Int32? InventoryID { get; set; }

        public abstract class subItemID : IBqlField { }

        [PXDefault]
        [SubItem(typeof(inventoryID))]
        public virtual Int32? SubItemID { get; set; }

        public abstract class qty : IBqlField { }
        
        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Qty")]
        public virtual decimal? Qty { get; set; }

        public abstract class qtyUOM : IBqlField { }

        [PXDBString]
        [PXDefault(typeof(Search<InventoryItem.baseUnit, 
                          Where<InventoryItem.inventoryID, Equal<Current<inventoryID>>>>), 
                          PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "UOM")]
        public virtual string QtyUOM { get; set; }

        public abstract class lotSerialNbr : IBqlField
        {
            public const int LENGTH = 100;
        }
        
        [LotSerialNbr]
        public virtual string LotSerialNbr { get; set; }

        public abstract class expireDate : IBqlField { }

        [PXDBDate]
        [PXUIField(DisplayName = "Expire Date")]
        public virtual DateTime? ExpireDate { get; set; }
    }
}
