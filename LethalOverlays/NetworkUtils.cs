using Unity.Netcode;

namespace com.github.zehsteam.LethalOverlays;

internal static class NetworkUtils
{
    public static bool IsServer
    {
        get
        {
            if (NetworkManager.Singleton == null) return false;

            return NetworkManager.Singleton.IsServer;
        }
    }

    public static bool IsHost
    {
        get
        {
            if (NetworkManager.Singleton == null) return false;

            return NetworkManager.Singleton.IsHost;
        }
    }

    public static ulong GetLocalClientId()
    {
        return NetworkManager.Singleton.LocalClientId;
    }

    public static bool IsLocalClientId(ulong clientId)
    {
        return clientId == GetLocalClientId();
    }
}
