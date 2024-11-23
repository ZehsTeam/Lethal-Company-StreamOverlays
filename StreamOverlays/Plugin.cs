using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using com.github.zehsteam.StreamOverlays.Dependencies;
using com.github.zehsteam.StreamOverlays.Patches;
using com.github.zehsteam.StreamOverlays.Server;
using HarmonyLib;
using System.Threading.Tasks;

namespace com.github.zehsteam.StreamOverlays;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(LethalConfigProxy.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
internal class Plugin : BaseUnityPlugin
{
    private readonly Harmony _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

    internal static Plugin Instance { get; private set; }
    internal static new ManualLogSource Logger { get; private set; }
    internal static new ConfigFile Config {  get; private set; }

    internal static ConfigManager ConfigManager { get; private set; }

    #pragma warning disable IDE0051 // Remove unused private members
    private void Awake()
    #pragma warning restore IDE0051 // Remove unused private members
    {
        if (Instance == null) Instance = this;

        Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} has awoken!");

        Config = Utils.CreateGlobalConfigFile();

        _harmony.PatchAll(typeof(StartOfRoundPatch));
        _harmony.PatchAll(typeof(TimeOfDayPatch));
        _harmony.PatchAll(typeof(PlayerControllerBPatch));
        _harmony.PatchAll(typeof(VehicleControllerPatch));
        _harmony.PatchAll(typeof(DepositItemsDeskPatch));

        ConfigManager = new ConfigManager();

        Task.Run(WebServer.Initialize);
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
