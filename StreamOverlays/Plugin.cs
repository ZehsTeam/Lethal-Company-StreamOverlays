using BepInEx;
using BepInEx.Logging;
using com.github.zehsteam.StreamOverlays.Dependencies;
using com.github.zehsteam.StreamOverlays.Patches;
using HarmonyLib;

namespace com.github.zehsteam.StreamOverlays;

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
        LogExtended(LogLevel.Info, data);
    }

    public void LogExtended(LogLevel level, object data)
    {
        if (ConfigManager == null || ConfigManager.ExtendedLogging == null)
        {
            Logger.Log(level, data);
            return;
        }

        if (ConfigManager.ExtendedLogging.Value)
        {
            Logger.Log(level, data);
        }
    }
}
