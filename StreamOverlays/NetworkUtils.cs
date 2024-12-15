using Unity.Netcode;

namespace com.github.zehsteam.StreamOverlays;

internal static class NetworkUtils
{
    public static bool IsConnected
    {
        get
        {
            if (NetworkManager.Singleton == null)
            {
                return false;
            }

            return NetworkManager.Singleton.IsConnectedClient;
        }
    }

    public static bool IsServer
    {
        get
        {
            if (NetworkManager.Singleton == null)
            {
                return false;
            }

            return NetworkManager.Singleton.IsServer;
        }
    }

    public static bool IsHost
    {
        get
        {
            if (NetworkManager.Singleton == null)
            {
                return false;
            }

            return NetworkManager.Singleton.IsHost;
        }
    }

    public static ulong GetLocalClientId()
    {
        if (NetworkManager.Singleton == null)
        {
            return 0;
        }

        return NetworkManager.Singleton.LocalClientId;
    }

    public static bool IsLocalClientId(ulong clientId)
    {
        return clientId == GetLocalClientId();
    }
}
