﻿using GameNetcodeStuff;

namespace com.github.zehsteam.LethalOverlays;

internal static class PlayerUtils
{
    public static PlayerControllerB GetLocalPlayerScript()
    {
        if (GameNetworkManager.Instance == null)
        {
            return null;
        }

        return GameNetworkManager.Instance.localPlayerController;
    }

    public static bool IsLocalPlayer(PlayerControllerB playerScript)
    {
        return playerScript == GetLocalPlayerScript();
    }

    public static bool IsLocalPlayerSpawned()
    {
        return GetLocalPlayerScript() != null;
    }

    public static PlayerControllerB GetPlayerScriptByClientId(ulong clientId)
    {
        foreach (var playerScript in StartOfRound.Instance.allPlayerScripts)
        {
            if (playerScript.actualClientId == clientId)
            {
                return playerScript;
            }
        }

        return null;
    }
}