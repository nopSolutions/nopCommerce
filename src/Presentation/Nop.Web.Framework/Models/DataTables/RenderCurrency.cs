namespace Nop.Web.Framework.Models.DataTables;

public partial class RenderCurrency : IRender
{
    private const Align ALIGN_DEFAULT = Align.Right;

    private const string CURRENCY_DEFAULT = "USD";

    public string Currency { get; set; }
    public Align TextAlign { get; set; }

    public RenderCurrency()
    {
        TextAlign = ALIGN_DEFAULT;
        Currency = CURRENCY_DEFAULT;
    }

    public enum Align
    {
        Left,
        Right,
        Center
    }
}
