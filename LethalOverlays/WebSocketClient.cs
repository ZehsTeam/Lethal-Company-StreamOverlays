using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine;
using WebSocketSharp;

namespace com.github.zehsteam.LethalOverlays;

internal static class WebSocketClient
{
    private static WebSocket _webSocket;

    // Node.js WebSocket server URL
    private static string _serverUrl => $"ws://127.0.0.1:{_serverPort}";
    private static int _serverPort
    {
        get
        {
            if (Plugin.ConfigManager != null && Plugin.ConfigManager.Networking_Port != null)
            {
                return Plugin.ConfigManager.Networking_Port.Value;
            }

            return 8080;
        }
    }
    private static bool _autoReconnect
    {
        get
        {
            if (Plugin.ConfigManager != null && Plugin.ConfigManager.Networking_AutoReconnect != null)
            {
                return Plugin.ConfigManager.Networking_AutoReconnect.Value;
            }

            return true;
        }
    }
    private static float _reconnectDelay
    {
        get
        {
            if (Plugin.ConfigManager != null && Plugin.ConfigManager.Networking_ReconnectDelay != null)
            {
                return Plugin.ConfigManager.Networking_ReconnectDelay.Value;
            }

            return 5f;
        }
    }

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

    // Connects to the Node.js WebSocket server
    private static void ConnectToServer()
    {
        _webSocket = new WebSocket(_serverUrl);

        _webSocket.OnOpen += (sender, e) =>
        {
            _isConnected = true;
            Plugin.Logger.LogInfo("Connected to Node.js server.");
            UpdateOverlay();
        };

        _webSocket.OnMessage += (sender, e) => ProcessServerMessage(e.Data);

        _webSocket.OnClose += (sender, e) =>
        {
            _isConnected = false;

            if (_autoReconnect)
            {
                Plugin.Logger.LogInfo("Disconnected from Node.js server. Attempting to reconnect...");
                Utils.StartCoroutine(ReconnectCoroutine());
            }
            else
            {
                Plugin.Logger.LogInfo("Disconnected from Node.js server.");
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

        if (!_isConnected)
        {
            Plugin.Logger.LogInfo("Reconnecting to Node.js server...");
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

    // Sends data to the Node.js server
    public static void SendData(object data)
    {
        if (_isConnected && _webSocket.ReadyState == WebSocketState.Open)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            _webSocket.Send(jsonData);
            Plugin.Instance.LogInfoExtended("Data sent to Node.js server: " + jsonData);
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
        // Send the latest data to Node.js
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
            day = Utils.GetDayCount(),
            quota = Utils.GetProfitQuota(),
            loot = Utils.GetLootTotal()
        };
        
        return latestData;
    }
}
