using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TheHuntIsOn.HkmpAddon;

[JsonConverter(typeof(StringEnumConverter))]
public enum NetItem
{
    VengefulSpirit,
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
    MaskShard,
    SoulVessel,
    NailUpgrade,
    Dreamer
}