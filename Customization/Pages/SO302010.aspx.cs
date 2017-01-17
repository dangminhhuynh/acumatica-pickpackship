using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using PX.Web.UI;
using PX.Objects.SO;
using System.Drawing;
using PX.Data;

public partial class Page_SO302010 : PX.Web.UI.PXPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterStyle("CssComplete", null, "Olive", true);
        RegisterStyle("CssPartial", null, "Olive", false);
        RegisterStyle("CssOverpick", null, "OrangeRed", true);
    }

    private void RegisterStyle(string name, string backColor, string foreColor, bool bold)
    {
        Style style = new Style();
        if (!string.IsNullOrEmpty(backColor)) style.BackColor = Color.FromName(backColor);
        if (!string.IsNullOrEmpty(foreColor)) style.ForeColor = Color.FromName(foreColor);
        if (bold) style.Font.Bold = true;
        this.Page.Header.StyleSheet.CreateStyleRule(style, this, "." + name);
    }

    protected void grid_RowDataBound(object sender, PX.Web.UI.PXGridRowEventArgs e)
    {
        SOShipLinePick shipLine = e.Row.DataItem as SOShipLinePick;
        if (shipLine == null) return;

        if(shipLine.PickedQty > shipLine.ShippedQty)
        {
            e.Row.Style.CssClass = "CssOverpick";
        }
        else if (shipLine.PickedQty == shipLine.ShippedQty)
        {
            e.Row.Style.CssClass = "CssComplete";
        }
        else if (shipLine.PickedQty > 0)
        {
            e.Row.Style.CssClass = "CssPartial";
        }
    }
}