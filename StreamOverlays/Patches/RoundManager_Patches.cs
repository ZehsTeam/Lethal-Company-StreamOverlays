using com.github.zehsteam.StreamOverlays.Helpers;
using com.github.zehsteam.StreamOverlays.Managers;
using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays.Patches;

[HarmonyPatch(typeof(RoundManager))]
internal static class RoundManager_Patches
{
    // This fixes non-host clients not assigning to scrapPersistedThroughRounds on GrabbableObjects
    [HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
    [HarmonyPostfix]
    private static void DespawnPropsAtEndOfRound_Patch(bool despawnAllItems)
    {
        LootManager.UpdateLootTotal();
        WebServer.UpdateOverlaysData();

        if (!despawnAllItems && !StartOfRound.Instance.allPlayersDead)
        {
            Utils.UpdateScrapPersistedThroughRounds();
        }
    }
}
