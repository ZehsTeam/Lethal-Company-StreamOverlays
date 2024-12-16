using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace com.github.zehsteam.StreamOverlays.Server;

public class OverlayBehavior : WebSocketBehavior
{
    protected override void OnOpen()
    {
        UpdateOverlayFormatting();
        UpdateOverlayData();
    }

    public void SendJsonToClient(object jsonData)
    {
        Send(JsonConvert.SerializeObject(jsonData));
    }

    public void UpdateOverlayFormatting()
    {
        SendJsonToClient(WebServer.GetOverlaysFormatting());
    }

    public void UpdateOverlayData()
    {
        SendJsonToClient(WebServer.GetOverlaysData());
    }
}
