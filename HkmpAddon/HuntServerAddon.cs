using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Hkmp.Api.Server;
using Newtonsoft.Json;

namespace TheHuntIsOn.HkmpAddon;

public class HuntServerAddon : ServerAddon
{
    #region Members

    private const string NetworkedEventsFileName = "networked-events.json";
    private const string DefaultNetworkedEventsFilePath = "TheHuntIsOn.Resources.default-networked-events.json";

    #endregion

    #region Properties

    protected override string Name => AddonIdentifier.Name;

    protected override string Version => AddonIdentifier.Version;

    public override bool NeedsNetwork => true;

    private ServerNetManager NetManager { get; set; }

    private Dictionary<NetEvent, ItemGrant> NetworkedEvents { get; set; } 

    #endregion

    #region Methods

    public override void Initialize(IServerApi serverApi)
    {
        NetManager = new ServerNetManager(this, serverApi.NetServer);

        if (!File.Exists(GetNetworkedEventsFilePath()))
        {
            Logger.Info($"Could not find networked items file: {NetworkedEventsFileName}, copying default");
            ExportDefaultNetworkedEvents();
        }

        LoadNetworkedEvents();

        NetManager.EventTriggeredEvent += OnEventTriggered;
    }

    private void OnEventTriggered(ushort id, NetEvent netEvent)
    {
        if (!NetworkedEvents.TryGetValue(netEvent, out var itemGrant))
        {
            return;
        }

        if (itemGrant.Items != null)
            NetManager.SendGrantItems(itemGrant.Items);

        if (itemGrant.Message != null)
            ServerApi.ServerManager.BroadcastMessage(itemGrant.Message);
    }

    private void ExportDefaultNetworkedEvents()
    {
        using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DefaultNetworkedEventsFilePath);
        using var fileStream = new FileStream(GetNetworkedEventsFilePath(), FileMode.Create, FileAccess.Write);

        resourceStream?.CopyTo(fileStream);
        fileStream.Flush();
    }

    private void LoadNetworkedEvents()
    {
        try
        {
            var fileContents = File.ReadAllText(GetNetworkedEventsFilePath());

            NetworkedEvents = JsonConvert.DeserializeObject<Dictionary<NetEvent, ItemGrant>>(fileContents);

            Logger.Info("Loaded networked events");
        }
        catch (Exception e)
        {
            Logger.Error($"Could not read networked events:\n{e}");
        }
    }

    private string GetNetworkedEventsFilePath()
    {
        var location = Assembly.GetExecutingAssembly().Location;
        var directory = Directory.GetParent(location)!;

        return Path.Combine(directory.FullName, NetworkedEventsFileName);
    } 

    #endregion
}