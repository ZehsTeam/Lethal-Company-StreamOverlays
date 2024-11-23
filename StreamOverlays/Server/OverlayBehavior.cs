using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace com.github.zehsteam.StreamOverlays.Server;

public class OverlayBehavior : WebSocketBehavior
{
    protected override void OnOpen()
    {
        UpdateOverlay();
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        try
        {
            var jsonObject = JObject.Parse(e.Data);

            if (jsonObject["request"]?.ToString() == "latestData")
            {
                UpdateOverlay();
            }
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"Failed to parse JSON message: {ex.Message}");
        }
    }

    public void SendJsonToClient(object jsonData)
    {
        Send(JsonConvert.SerializeObject(jsonData));
    }

    public void UpdateOverlay()
    {
        SendJsonToClient(WebServer.GetOverlayData());
    }
}
