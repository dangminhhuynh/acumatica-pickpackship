using System;
using PX.Data;

namespace PX.SM
{
    [Serializable]
    public class SOPickPackShipPrintSetup : IBqlTable
    {
        public abstract class userID : PX.Data.IBqlField { }
        [PXDBGuid]
        [PXDefault(typeof(Search<Users.pKID, Where<Users.pKID, Equal<Current<AccessInfo.userID>>>>))]
        [PXUIField(DisplayName = "User")]
        public virtual Guid? UserID { get; set; }

        public abstract class shipmentConfirmation : PX.Data.IBqlField { }
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Print Shipment Confirmation")]
        public virtual bool? ShipmentConfirmation { get; set; }

        public abstract class shipmentConfirmationQueue : PX.Data.IBqlField { }
        [PXDBString(10)]
        [PXSelector(typeof(SMPrintQueue.printQueue))]
        [PXUIEnabled(typeof(shipmentConfirmation))]
        [PXUIField(DisplayName = "Print Queue")]
        public virtual string ShipmentConfirmationQueue { get; set; }

        public abstract class shipmentLabels : PX.Data.IBqlField { }
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Print Shipment Labels")]
        public virtual bool? ShipmentLabels { get; set; }

        public abstract class shipmentLabelsQueue : PX.Data.IBqlField { }
        [PXDBString(10)]
        [PXSelector(typeof(SMPrintQueue.printQueue))]
        [PXUIEnabled(typeof(shipmentLabels))]
        [PXUIField(DisplayName = "Print Queue")]
        public virtual string ShipmentLabelsQueue { get; set; }
    }
}
