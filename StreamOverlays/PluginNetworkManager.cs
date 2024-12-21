using System.Linq;
using Unity.Collections;
using Unity.Netcode;

namespace com.github.zehsteam.StreamOverlays;

internal static class PluginNetworkManager
{
    public static void Initialize()
    {
        if (NetworkManager.Singleton == null)
        {
            Plugin.Logger.LogError("Failed to initialize PluginNetworkManager. NetworkManager.Singleton is null.");
            return;
        }

        if (NetworkManager.Singleton.CustomMessagingManager == null)
        {
            Plugin.Logger.LogError("Failed to initialize PluginNetworkManager. NetworkManager.Singleton.CustomMessagingManager is null.");
            return;
        }

        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ReceiveCustomMessage", HandleCustomMessage);

        Plugin.Logger.LogInfo("PluginNetworkManager initialized successfully.");
    }

    public static void OnClientConnected(ulong clientId)
    {
        if (!NetworkUtils.IsServer) return;

        try
        {
            CustomMessage customMessage = new CustomMessage
            {
                DaysSpent = StartOfRound.Instance.gameStats.daysSpent,
                TimesFulfilledQuota = TimeOfDay.Instance.timesFulfilledQuota,
                DayDataList = DayManager.DayDataList.ToArray()
            };

            SendCustomMessageToClient(clientId, customMessage);
        }
        catch (System.Exception ex)
        {
            Plugin.Logger.LogError($"Failed to send CustomMessage data to client: {clientId}. {ex}");
        }
    }

    private static void SendCustomMessageToClient(ulong clientId, CustomMessage customMessage)
    {
        if (!NetworkUtils.IsServer)
        {
            Plugin.Logger.LogWarning("Only the host can send messages to clients.");
            return;
        }

        try
        {
            using FastBufferWriter writer = new FastBufferWriter(1024, Allocator.Temp);

            // Write the CustomMessage fields
            writer.WriteValueSafe(customMessage.DaysSpent);
            writer.WriteValueSafe(customMessage.TimesFulfilledQuota);

            // Write the DayData array length
            writer.WriteValueSafe(customMessage.DayDataList.Length);

            // Write each DayData entry
            foreach (var dayData in customMessage.DayDataList)
            {
                writer.WriteValueSafe(dayData.Day);
                writer.WriteValueSafe(dayData.ScrapCollected);
            }

            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ReceiveCustomMessage", clientId, writer);

            Plugin.Logger.LogInfo($"Sent CustomMessage data to client: {clientId}");
        }
        catch (System.Exception ex)
        {
            Plugin.Logger.LogError($"Failed to send CustomMessage data to client: {clientId}. {ex}");
        }
    }

    private static void HandleCustomMessage(ulong senderId, FastBufferReader reader)
    {
        if (!reader.TryBeginRead(sizeof(int) * 2)) // Ensure there's enough data to read
        {
            Plugin.Logger.LogWarning("Failed to deserialize message: Insufficient data.");
            return;
        }

        // Deserialize the message
        try
        {
            // Read the CustomMessage fields
            CustomMessage customMessage = new CustomMessage();

            reader.ReadValueSafe(out customMessage.DaysSpent);
            reader.ReadValueSafe(out customMessage.TimesFulfilledQuota);

            // Read the DayData array length
            int dayDataListLength;
            reader.ReadValueSafe(out dayDataListLength);

            // Initialize the array
            customMessage.DayDataList = new DayData[dayDataListLength];

            for (int i = 0; i < dayDataListLength; i++)
            {
                customMessage.DayDataList[i] = new DayData();
                reader.ReadValueSafe(out customMessage.DayDataList[i].Day);
                reader.ReadValueSafe(out customMessage.DayDataList[i].ScrapCollected);
            }

            // Handle the message (e.g., update client-side state)
            Plugin.Logger.LogInfo($"Received message from client: {senderId}");

            ApplyCustomMessageData(customMessage);
        }
        catch (System.Exception ex)
        {
            Plugin.Logger.LogError($"Failed to deserialize message. {ex}");
        }
    }

    private static void ApplyCustomMessageData(CustomMessage customMessage)
    {
        if (customMessage == null)
        {
            Plugin.Logger.LogError("Failed to apply CustomMessage data. CustomMessage is null.");
            return;
        }

        try
        {
            StartOfRound.Instance.gameStats.daysSpent = customMessage.DaysSpent;
            TimeOfDay.Instance.timesFulfilledQuota = customMessage.TimesFulfilledQuota;
            DayManager.DayDataList = customMessage.DayDataList.ToList();

            Plugin.Logger.LogInfo("Applied CustomMessage data successfully!");
        }
        catch (System.Exception ex)
        {
            Plugin.Logger.LogError($"Failed to apply CustomMessage data. {ex}");
        }
    }
}

[System.Serializable]
public class CustomMessage
{
    public int DaysSpent;
    public int TimesFulfilledQuota;
    public DayData[] DayDataList;
}
