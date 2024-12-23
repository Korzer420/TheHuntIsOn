using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TheHuntIsOn.HkmpAddon;

[JsonConverter(typeof(StringEnumConverter))]
public enum NetEvent
{
    VengefulSpirit = 0,
    DesolateDive,
    HowlingWraiths,
    ShadeSoul,
    DescendingDark,
    AbyssShriek,
    MothwingCloak,
    MantisClaw,
    CrystalHeart,
    MonarchWings,
    IsmasTear,
    ShadeCloak,
    DreamNail,
    CycloneSlash,
    DashSlash,
    GreatSlash,
    Mask,
    SoulVessel,
    Movement1,
    Movement2,
    Movement3,
    Movement4,
    Movement5,
    Movement6,
    Dreamer,
    Tram,
    Stag,
    Toll,
    Grub,
    DreamWarrior,
    ShopPurchase,
    NailUpgrade,
    RelicSale,
    LeverHit,
    Notch4,
    Notch5,
    Notch6,
    Notch7,
    Notch8,
    Notch9,
    Notch10,
    Notch11
}