using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine;
using WebSocketSharp;

namespace com.github.zehsteam.LethalOverlays;

internal static class WebSocketClient
{
    private static WebSocket _webSocket;

    // WebSocket server URL
    private static string _serverUrl => $"ws://{_serverIP}:{_serverPort}";
    private static string _serverIP => Plugin.ConfigManager.Networking_IP.Value;
    private static int _serverPort => Plugin.ConfigManager.Networking_Port.Value;
    private static bool _autoReconnect => Plugin.ConfigManager.Networking_AutoReconnect.Value;
    private static float _reconnectDelay => Plugin.ConfigManager.Networking_ReconnectDelay.Value;

    private static bool _isConnected = false;

    // Initializes the WebSocket connection
    public static void Initialize()
    {
        Application.quitting += CloseConnection;

        if (_webSocket == null)
        {
            ConnectToServer();
        }
    }

    // Connects to the WebSocket server
    private static void ConnectToServer()
    {
        if (!Plugin.ConfigManager.Networking_Enabled.Value)
        {
            Plugin.Logger.LogInfo("Cancelled connection to server. Networking is disabled in the config settings.");
            return;
        }

        _webSocket = new WebSocket(_serverUrl);

        _webSocket.OnOpen += (sender, e) =>
        {
            _isConnected = true;
            Plugin.Logger.LogInfo("Connected to server.");
            UpdateOverlay();
        };

        _webSocket.OnMessage += (sender, e) => ProcessServerMessage(e.Data);

        _webSocket.OnClose += (sender, e) =>
        {
            _isConnected = false;

            if (_autoReconnect)
            {
                Plugin.Logger.LogInfo("Disconnected from server. Attempting to reconnect...");
                Utils.StartCoroutine(ReconnectCoroutine());
            }
            else
            {
                Plugin.Logger.LogInfo("Disconnected from server.");
            }
        };

        _webSocket.OnError += (sender, e) =>
        {
            Plugin.Logger.LogError($"WebSocket error: {e.Message}");
        };

        _webSocket.ConnectAsync(); // Start asynchronous connection
    }

    // Coroutine to reconnect after a delay
    private static IEnumerator ReconnectCoroutine()
    {
        yield return new WaitForSeconds(_reconnectDelay);

        if (!_autoReconnect)
        {
            yield break;
        }

        if (!_isConnected)
        {
            Plugin.Logger.LogInfo("Reconnecting to server...");
            ConnectToServer();
        }
    }

    // Manually reconnects to the server
    public static void Reconnect()
    {
        if (_webSocket != null)
        {
            _webSocket.Close();
        }

        ConnectToServer();
    }

    // Sends data to the server
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
            Plugin.Logger.LogWarning("Unable to send data. WebSocket is not connected.");
        }
    }

    // Processes incoming messages from the server
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
            Plugin.Logger.LogError($"Failed to parse JSON message: {e.Message}");
        }
    }

    // Closes the WebSocket connection (e.g., on application quit)
    public static void CloseConnection()
    {
        if (_webSocket != null)
        {
            _webSocket.Close();
            _webSocket = null;
            _isConnected = false;
            Plugin.Logger.LogInfo("WebSocket connection closed.");
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
