﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace com.github.zehsteam.StreamOverlays.Server;

internal static class WebServer
{
    public static int HttpPort => Plugin.ConfigManager.Server_HttpPort.Value;
    public static int WebSocketPort => Plugin.ConfigManager.Server_WebSocketPort.Value;
    public static bool IsRunning => _isRunning;

    private static HttpListener _httpListener;
    private static WebSocketServer _webSocketServer;
    private static bool _isRunning;

    public static async Task Initialize()
    {
        Application.quitting += Stop;

        if (WebsiteArchiveExists())
        {
            await DecompressWebsiteArchive();
        }

        if (!WebsiteFolderExists())
        {
            Plugin.Logger.LogFatal("Error! The \"public\" folder does not exist. The overlays will not work. Please report the bug to the mod developer!");
        }

        if (Plugin.ConfigManager.Server_AutoStart.Value)
        {
            Start();
        }
    }
    
    public static void Start()
    {
        if (_isRunning)
        {
            Plugin.Logger.LogWarning("Server is already running!");
            return;
        }

        _isRunning = true;

        // Start HTTP Server
        Task.Run(StartHttpServer);

        // Start WebSocket Server
        _webSocketServer = new WebSocketServer($"ws://{System.Net.IPAddress.Any}:{WebSocketPort}");
        _webSocketServer.AddWebSocketService<OverlayBehavior>("/overlay");
        _webSocketServer.Start();

        Plugin.Logger.LogInfo($"WebSocket server started on ws://localhost:{WebSocketPort}");
    }

    public static void Stop()
    {
        if (!_isRunning) return;

        _isRunning = false;

        _httpListener?.Stop();
        _webSocketServer?.Stop();

        Plugin.Logger.LogInfo("Server stopped.");
    }

    private static void StartHttpServer()
    {
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add($"http://{System.Net.IPAddress.Any}:{HttpPort}/");
        _httpListener.Start();

        Plugin.Logger.LogInfo($"HTTP server started on http://localhost:{HttpPort}");
        
        while (_isRunning)
        {
            try
            {
                var context = _httpListener.GetContext();
                Task.Run(() => HandleHttpRequest(context));
            }
            catch (Exception ex) when (!_isRunning)
            {
                Plugin.Logger.LogWarning($"HTTP server stopped: {ex.Message}");
            }
        }
    }

    private static void HandleHttpRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        try
        {
            string requestedPath = request.Url.LocalPath.TrimStart('/'); // e.g., "overlay" or "assets/css/style.css"

            Plugin.Instance.LogInfoExtended($"Requested path: \"{requestedPath}\"");

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

            Plugin.Logger.LogError($"Error serving request: {ex.Message}");
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
        if (_webSocketServer == null)
        {
            return;
        }

        var json = JsonConvert.SerializeObject(jsonData);

        foreach (var path in _webSocketServer.WebSocketServices.Paths)
        {
            // Access the service host for the path
            var serviceHost = _webSocketServer.WebSocketServices[path];

            // Broadcast to all connected sessions
            serviceHost.Sessions.Broadcast(json);
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

            crewLabel = Plugin.ConfigManager.CrewStat_Label.Value,
            moonLabel = Plugin.ConfigManager.MoonStat_Label.Value,
            dayLabel = Plugin.ConfigManager.DayStat_Label.Value,
            quotaLabel = Plugin.ConfigManager.QuotaStat_Label.Value,
            lootLabel = Plugin.ConfigManager.LootStat_Label.Value,
            averagePerDayLabel = Plugin.ConfigManager.AveragePerDayStat_Label.Value
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
            weatherName = Utils.GetEnumName(Utils.GetCurrentPlanetWeather()),
            showWeatherIcon = Plugin.ConfigManager.MoonStat_ShowWeatherIcon.Value,
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

            Plugin.Logger.LogInfo("Successfully decompressed public archive.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"Error while decompressing public archive: {ex.Message}");
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
