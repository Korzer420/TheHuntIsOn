using Hkmp.Api.Client;
using TheHuntIsOn.Modules;

namespace TheHuntIsOn.HkmpAddon;

public class HuntClientAddon : ClientAddon
{
    #region Properties

    public ClientNetManager NetManager { get; private set; }

    protected override string Name => AddonIdentifier.Name;

    protected override string Version => AddonIdentifier.Version;

    public override bool NeedsNetwork => true;


    #endregion

    #region Methods

    public override void Initialize(IClientApi clientApi)
    {
        NetManager = new ClientNetManager(this, clientApi.NetClient);
        EventNetworkModule._clientApi = clientApi;
    }

    #endregion
}