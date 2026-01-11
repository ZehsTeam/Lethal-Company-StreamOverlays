using com.github.zehsteam.StreamOverlays.Helpers;
using com.github.zehsteam.StreamOverlays.Managers;
using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(RoundManager))]
internal static class RoundManagerPatch
{
    [HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
    [HarmonyPostfix]
    private static void DespawnPropsAtEndOfRoundPatch()
    {
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();
    }

    // This fixes non-host clients not assigning to scrapPersistedThroughRounds on GrabbableObjects
    [HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
    [HarmonyPostfix]
    private static void DespawnPropsAtEndOfRoundPatch(bool despawnAllItems)
    {
        if (despawnAllItems || StartOfRound.Instance.allPlayersDead)
        {
            return;
        }

        Utils.UpdateScrapPersistedThroughRounds();
    }
}
