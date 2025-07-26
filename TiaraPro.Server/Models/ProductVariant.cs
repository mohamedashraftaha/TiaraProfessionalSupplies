using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TiaraPro.Server.Models;

public class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string SKU { get; set; } = null!;
    public int Quantity { get; set; }

    public ProductSide? Side { get; set; } = null;

    public int? Size { get; set; } = null;

    public int Range { get; set; } = 7;

    public bool OnlyOneOption { get; set; } = false;

    public VariableOptions? VariableOption { get; set; } = null;

    public int ParentProductId { get; set; }
}

public enum ProductSide
{
    UpperRight,
    LowerRight,
    LowerLeft,
    UpperLeft,
    PrimaryCentral,
    PrimaryLateral,
    PrimaryCuspid,
    PrimaryUpperCuspid,
    PrimaryLowerCuspid
}

public enum VariableOptions
{
    [Description("9061100 Extrathin")]
    Extrathin_9061100,

    [Description("9061101 13T")]
    ThirteenT_9061101,

    [Description("9061102 13")]
    Thirteen_9061102,

    [Description("9061106 Ultra thin")]
    UltraThin_9061106,

    [Description("9061109 Mini")]
    Mini_9061109,

    [Description("Blue")]
    Blue
}
