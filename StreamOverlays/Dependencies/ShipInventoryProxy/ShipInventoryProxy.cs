using BepInEx.Bootstrap;
using com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy.Patches;
using HarmonyLib;
using ShipInventory.Helpers;
using System.Runtime.CompilerServices;

namespace com.github.zehsteam.StreamOverlays.Dependencies.ShipInventoryProxy;

internal static class ShipInventoryProxy
{
    public const string PLUGIN_GUID = ShipInventory.MyPluginInfo.PLUGIN_GUID;
    public static bool Enabled => Chainloader.PluginInfos.ContainsKey(PLUGIN_GUID);

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void PatchAll(Harmony harmony)
    {
        harmony.PatchAll(typeof(ItemManagerPatch));
        harmony.PatchAll(typeof(ChuteInteractPatch));
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static int GetLootTotal()
    {
        return ItemManager.GetTotalValue();
    }
}
