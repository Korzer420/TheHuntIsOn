using Newtonsoft.Json;
using System.Collections.Generic;

namespace TheHuntIsOn.HkmpAddon;

internal class ItemGrant
{
    #region Properties

    [JsonProperty("items")]
    public List<NetItem> Items { get; set; }
    [JsonProperty("message")]
    public string Message { get; set; }

    #endregion
}
