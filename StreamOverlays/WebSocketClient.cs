using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine;
using WebSocketSharp;

namespace com.github.zehsteam.StreamOverlays;

internal static class WebSocketClient
{
    private static WebSocket _webSocket;

    private static string _serverUrl => $"ws://{_serverIP}:{_serverPort}";
    private static string _serverIP => Plugin.ConfigManager.Client_ServerIP.Value;
    private static int _serverPort => Plugin.ConfigManager.Client_ServerPort.Value;
    private static bool _autoReconnect => Plugin.ConfigManager.Client_AutoReconnect.Value;
    private static float _reconnectDelay => Plugin.ConfigManager.Client_ReconnectDelay.Value;

    private static bool _isConnected = false;

    public static void Initialize()
    {
        Application.quitting += CloseConnection;

        if (_webSocket == null)
        {
            ConnectToServer();
        }
    }

    private static void ConnectToServer()
    {
        if (!Plugin.ConfigManager.Client_Enabled.Value)
        {
            Plugin.Logger.LogInfo("Cancelled connection to server. Networking is disabled in the config settings.");
            return;
        }

        _webSocket = new WebSocket(_serverUrl);

        _webSocket.Log.Output = (data, path) =>
        {
            // Map WebSocketSharp.LogLevel to BepInEx.Logging.LogLevel
            BepInEx.Logging.LogLevel logLevel = data.Level switch
            {
                WebSocketSharp.LogLevel.Trace => BepInEx.Logging.LogLevel.Info,
                WebSocketSharp.LogLevel.Debug => BepInEx.Logging.LogLevel.Debug,
                WebSocketSharp.LogLevel.Info => BepInEx.Logging.LogLevel.Info,
                WebSocketSharp.LogLevel.Warn => BepInEx.Logging.LogLevel.Warning,
                WebSocketSharp.LogLevel.Error => BepInEx.Logging.LogLevel.Error,
                WebSocketSharp.LogLevel.Fatal => BepInEx.Logging.LogLevel.Fatal,
                _ => BepInEx.Logging.LogLevel.Info, // Default to Info if unrecognized
            };

            // Log the message using the mapped log level
            Plugin.Instance.LogExtended(logLevel, data.Message);
        };

        _webSocket.OnOpen += (sender, e) =>
        {
            _isConnected = true;

            Plugin.Logger.LogInfo("Connected to server.");

            UpdateOverlay();
        };

        _webSocket.OnMessage += (sender, e) => ProcessServerMessage(e.Data);

        _webSocket.OnClose += (sender, e) =>
        {
            if (_isConnected)
            {
                Plugin.Logger.LogInfo("Disconnected from server.");
            }

            _isConnected = false;

            if (_autoReconnect)
            {
                Plugin.Instance.LogInfoExtended("Attempting to reconnect...");

                Utils.StartCoroutine(ReconnectCoroutine());
            }
        };

        _webSocket.OnError += (sender, e) =>
        {
            Plugin.Instance.LogErrorExtended($"WebSocket error: {e.Message}");
        };

        _webSocket.ConnectAsync(); // Start asynchronous connection
    }

    private static IEnumerator ReconnectCoroutine()
    {
        yield return new WaitForSeconds(_reconnectDelay);

        if (!_autoReconnect)
        {
            yield break;
        }

        if (!_isConnected)
        {
            Plugin.Instance.LogInfoExtended("Reconnecting to server...");
            ConnectToServer();
        }
    }

    public static void Reconnect()
    {
        if (_webSocket != null)
        {
            _webSocket.Close();
        }

        ConnectToServer();
    }

    public static void SendData(object data)
    {
        if (_isConnected && _webSocket.ReadyState == WebSocketState.Open)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            _webSocket.Send(jsonData);
            Plugin.Instance.LogInfoExtended("Data sent to server: " + jsonData);
        }
        else
        {
            Plugin.Instance.LogWarningExtended("Unable to send data. WebSocket is not connected.");
        }
    }

    private static void ProcessServerMessage(string message)
    {
        Plugin.Instance.LogInfoExtended("Message received from server: " + message);

        try
        {
            // Parse the message as a JObject
            var jsonObject = JObject.Parse(message);

            // Check if the "request" key exists and its value is "latestData"
            if (jsonObject["request"]?.ToString() == "latestData")
            {
                UpdateOverlay();
            }
        }
        catch (JsonReaderException e)
        {
            Plugin.Instance.LogErrorExtended($"Failed to parse JSON message: {e.Message}");
        }
    }

    public static void CloseConnection()
    {
        if (_webSocket != null)
        {
            _webSocket.Close();
            _webSocket = null;
            _isConnected = false;
        }
    }

    public static void UpdateOverlay()
    {
        // Send the latest data to server
        SendData(GetLatestData());
    }

    public static object GetLatestData()
    {
        // Gather the latest game data
        var latestData = new
        {
            source = "overlay",
            visible = Utils.CanShowOverlay(),
            crew = Utils.GetCrewCount(),
            moon = Utils.GetCurrentPlanetName(),
            weather = Utils.GetEnumName(Utils.GetCurrentPlanetWeather()),
            showWeatherIcon = Plugin.ConfigManager.Overlay_ShowWeatherIcon.Value,
            day = Utils.GetDayCount(),
            quota = Utils.GetProfitQuota(),
            loot = Utils.GetLootTotal()
        };
        
        return latestData;
    }
}
