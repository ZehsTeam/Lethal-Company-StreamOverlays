using Unity.Netcode;

namespace com.github.zehsteam.StreamOverlays.Helpers;

internal static class NetworkUtils
{
    public static bool IsConnected => NetworkManager.Singleton?.IsConnectedClient ?? false;
    public static bool IsServer => NetworkManager.Singleton?.IsServer ?? false;

    public static ulong GetLocalClientId()
    {
        return NetworkManager.Singleton?.LocalClientId ?? 0;
    }

    public static bool IsLocalClientId(ulong clientId)
    {
        return clientId == GetLocalClientId();
    }
}
