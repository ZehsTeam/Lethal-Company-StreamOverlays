using BepInEx;
using BepInEx.Logging;
using com.github.zehsteam.LethalOverlays.Dependencies;
using com.github.zehsteam.LethalOverlays.Patches;
using HarmonyLib;

namespace com.github.zehsteam.LethalOverlays;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(LethalConfigProxy.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
internal class Plugin : BaseUnityPlugin
{
    private readonly Harmony _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

    internal static Plugin Instance { get; private set; }
    internal static new ManualLogSource Logger { get; private set; }

    internal static ConfigManager ConfigManager { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;

        Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} has awoken!");

        _harmony.PatchAll(typeof(StartOfRoundPatch));
        _harmony.PatchAll(typeof(TimeOfDayPatch));
        _harmony.PatchAll(typeof(PlayerControllerBPatch));
        _harmony.PatchAll(typeof(VehicleControllerPatch));
        _harmony.PatchAll(typeof(DepositItemsDeskPatch));

        ConfigManager = new ConfigManager();

        WebSocketClient.Initialize();
    }
    
    public void LogInfoExtended(object data)
    {
        if (ConfigManager.ExtendedLogging.Value)
        {
            Logger.LogInfo(data);
        }
    }

    public void LogMessageExtended(object data)
    {
        if (ConfigManager.ExtendedLogging.Value)
        {
            Logger.LogMessage(data);
        }
    }
}
