using com.github.zehsteam.StreamOverlays.Helpers;
using com.github.zehsteam.StreamOverlays.Managers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace com.github.zehsteam.StreamOverlays.Server;

internal static class WebServer
{
    public static int HttpPort => ConfigManager.Server_HttpPort.Value;
    public static int WebSocketPort => ConfigManager.Server_WebSocketPort.Value;
    public static bool IsRunning { get; private set; }

    private static HttpListener _httpListener;
    private static WebSocketServer _webSocketServer;

    private static readonly object _broadcastLock = new object();
    private static bool _isApplicationQuitting;

    public static async Task Initialize()
    {
        Application.quitting += () =>
        {
            _isApplicationQuitting = true;
            Stop();
        };

        if (WebsiteArchiveExists())
        {
            await DecompressWebsiteArchive();
        }

        if (!WebsiteFolderExists())
        {
            Logger.LogFatal("Error! The \"public\" folder does not exist. The overlays will not work. Please report the bug to the mod developer!");
        }

        if (ConfigManager.Server_AutoStart.Value)
        {
            Start();
        }
    }

    public static void Start()
    {
        if (IsRunning)
        {
            Logger.LogWarning("Server is already running!");
            return;
        }

        try
        {
            IsRunning = true;

            // Start HTTP Server and wait for completion
            if (!StartHttpServer())
            {
                Logger.LogError("Failed to start WebSocket server. HTTP server failed to start.");
                Stop();
                return;
            }

            // Start WebSocket Server
            _webSocketServer = new WebSocketServer($"ws://{System.Net.IPAddress.Any}:{WebSocketPort}");
            _webSocketServer.AddWebSocketService<OverlayBehavior>("/overlay");
            _webSocketServer.Start();

            if (!_webSocketServer.IsListening)
            {
                Logger.LogError("Failed to start WebSocket server. The port might already be in use.");
                Stop();
                return;
            }

            Logger.LogInfo($"WebSocket server started on ws://localhost:{WebSocketPort}");
        }
        catch (SocketException ex)
        {
            Logger.LogError($"Failed to start WebSocket server. {ex.Message}");
            Stop();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to start WebSocket server. {ex}");
            Stop();
        }
    }

    public static void Stop()
    {
        if (!IsRunning) return;

        IsRunning = false;

        if (_httpListener != null)
        {
            _httpListener.Stop();
            _httpListener.Close();
            _httpListener = null;
        }

        _webSocketServer?.Stop();
        _webSocketServer = null;

        Logger.LogInfo("Server stopped.");
    }

    private static bool StartHttpServer()
    {
        try
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://{System.Net.IPAddress.Any}:{HttpPort}/");
            _httpListener.Start();

            Logger.LogInfo($"HTTP server started on http://localhost:{HttpPort}");

            // Run the HTTP server loop in a background thread
            _ = Task.Run(HandleHttpRequests);

            return true;
        }
        catch (SocketException ex)
        {
            Logger.LogError($"Failed to start HTTP server. {ex.Message}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to start HTTP server. {ex}");
        }

        return false;
    }

    private static void HandleHttpRequests()
    {
        while (IsRunning && _httpListener != null)
        {
            try
            {
                var result = _httpListener.BeginGetContext(OnHttpRequest, _httpListener);
                result.AsyncWaitHandle.WaitOne(); // Wait for the next request
            }
            catch (Exception ex) when (IsRunning)
            {
                Logger.LogError($"Error handling HTTP requests: {ex.Message}");
            }
        }
    }

    private static void OnHttpRequest(IAsyncResult asyncResult)
    {
        if (_httpListener == null || !_httpListener.IsListening || _isApplicationQuitting)
        {
            return;
        }

        var context = _httpListener.EndGetContext(asyncResult);

        Task.Run(() =>
        {
            try
            {
                HandleHttpRequest(context);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error processing HTTP request: {ex.Message}");
            }
        });
    }

    private static void HandleHttpRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        try
        {
            string requestedPath = request.Url.LocalPath.TrimStart('/'); // e.g., "overlay" or "assets/css/style.css"

            Logger.LogInfo($"Requested path: \"{requestedPath}\"", extended: true);

            if (string.IsNullOrEmpty(requestedPath))
            {
                requestedPath = "overlay.html";
            }
            else if (!Path.HasExtension(requestedPath))
            {
                requestedPath += ".html";
            }

            if (requestedPath.Equals("config.js", StringComparison.OrdinalIgnoreCase))
            {
                response.ContentType = "application/javascript";
                string content = $"const webSocketPort = {WebSocketPort};";
                byte[] contentBytes = Encoding.UTF8.GetBytes(content);
                response.ContentLength64 = contentBytes.Length;
                response.OutputStream.Write(contentBytes, 0, contentBytes.Length);
                return;
            }

            // Define the base directory for your website files
            string baseDirectory = Path.Combine(Utils.GetPluginDirectoryPath(), "public");

            // Combine the base directory with the requested path
            string filePath = Path.Combine(baseDirectory, requestedPath);

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Determine the MIME type for the response
                string mimeType = GetMimeType(filePath);
                response.ContentType = mimeType;

                // Read and send the file
                byte[] fileBytes = File.ReadAllBytes(filePath);
                response.ContentLength64 = fileBytes.Length;
                response.OutputStream.Write(fileBytes, 0, fileBytes.Length);
            }
            else
            {
                // File not found, return a 404 response
                response.StatusCode = (int)HttpStatusCode.NotFound;
                byte[] errorBytes = Encoding.UTF8.GetBytes("404 - File Not Found");
                response.ContentLength64 = errorBytes.Length;
                response.OutputStream.Write(errorBytes, 0, errorBytes.Length);
            }
        }
        catch (Exception ex)
        {
            // Handle server errors
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            byte[] errorBytes = Encoding.UTF8.GetBytes("500 - Internal Server Error");
            response.ContentLength64 = errorBytes.Length;
            response.OutputStream.Write(errorBytes, 0, errorBytes.Length);

            Logger.LogError($"Error serving request: {ex.Message}");
        }
        finally
        {
            response.OutputStream.Close();
        }
    }

    private static string GetMimeType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".woff" => "font/woff",
            ".woff2" => "font/woff2",
            ".ttf" => "font/ttf",
            ".otf" => "font/otf",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".json" => "application/json",
            _ => "application/octet-stream", // Default MIME type
        };
    }

    public static void SendJsonToClients(object jsonData)
    {
        if (_webSocketServer == null || !_webSocketServer.IsListening)
        {
            return;
        }

        lock (_broadcastLock)
        {
            try
            {
                var json = JsonConvert.SerializeObject(jsonData);

                foreach (var path in _webSocketServer.WebSocketServices.Paths)
                {
                    var serviceHost = _webSocketServer.WebSocketServices[path];
                    serviceHost.Sessions.Broadcast(json);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to send JSON data to WebSocket clients. {ex}");
            }
        }
    }

    public static void UpdateOverlays()
    {
        UpdateOverlaysFormatting();
        UpdateOverlaysData();
    }

    public static void UpdateOverlaysFormatting()
    {
        SendJsonToClients(GetOverlaysFormatting());
    }

    public static void UpdateOverlaysData()
    {
        SendJsonToClients(GetOverlaysData());
    }

    public static object GetOverlaysFormatting()
    {
        return new
        {
            type = "formatting",

            crewLabel = ConfigManager.CrewStat_Label.Value,
            moonLabel = ConfigManager.MoonStat_Label.Value,
            dayLabel = ConfigManager.DayStat_Label.Value,
            quotaLabel = ConfigManager.QuotaStat_Label.Value,
            lootLabel = ConfigManager.LootStat_Label.Value,
            averagePerDayLabel = ConfigManager.AveragePerDayStat_Label.Value
        };
    }

    public static object GetOverlaysData()
    {
        return new
        {
            type = "data",

            showOverlay = Utils.CanShowOverlay(),
            crewCount = Utils.GetPlayerCount(),
            moonName = Utils.GetCurrentPlanetName(),
            weatherName = Utils.GetCurrentPlanetWeather().ToString(),
            showWeatherIcon = ConfigManager.MoonStat_ShowWeatherIcon.Value,
            dayCount = DayManager.GetDayNumber(),
            dayInQuota = Utils.GetDayInQuota(),
            quotaValue = Utils.GetProfitQuota(),
            quotaIndex = Utils.GetQuotaIndex(),
            lootValue = LootManager.GetLootTotal(),
            averagePerDayValue = DayManager.GetAveragePerDay()
        };
    }

    #region Website Folder/Archive
    private static bool WebsiteFolderExists()
    {
        return Directory.Exists(GetWebsiteFolderPath());
    }

    private static bool WebsiteArchiveExists()
    {
        return File.Exists(GetWebsiteArchivePath());
    }

    private static async Task DecompressWebsiteArchive()
    {
        string archivePath = GetWebsiteArchivePath();

        if (!File.Exists(archivePath))
        {
            return;
        }

        try
        {
            string extractPath = Utils.GetPluginDirectoryPath();

            await Task.Run(() =>
            {
                using var archive = ZipFile.OpenRead(archivePath);

                foreach (var entry in archive.Entries)
                {
                    string destinationPath = Path.Combine(extractPath, entry.FullName);
                    string destinationDir = Path.GetDirectoryName(destinationPath);

                    if (!string.IsNullOrEmpty(destinationDir))
                    {
                        Directory.CreateDirectory(destinationDir);
                    }

                    if (!entry.FullName.EndsWith("/"))
                    {
                        entry.ExtractToFile(destinationPath, overwrite: true);
                    }
                }
            });

            File.Delete(archivePath);

            Logger.LogInfo("Successfully decompressed public archive.");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error while decompressing public archive: {ex.Message}");
        }
    }

    private static string GetWebsiteFolderPath()
    {
        return Path.Combine(Utils.GetPluginDirectoryPath(), "public");
    }

    private static string GetWebsiteArchivePath()
    {
        return Path.Combine(Utils.GetPluginDirectoryPath(), "public.zip");
    }
    #endregion
}
