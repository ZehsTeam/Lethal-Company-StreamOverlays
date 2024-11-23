using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace com.github.zehsteam.StreamOverlays;

internal static class ServerHelper
{
    public const string FolderName = "Server";
    public const string ArchiveName = "server";
    public const string ExecutableName = "server";

    private static int _serverPort => Plugin.ConfigManager.Server_Port.Value;

    public static async void Initialize()
    {
        if (IsPacked())
        {
            await Unpack();
        }

        if (Plugin.ConfigManager.Server_AutoStart.Value)
        {
            Start();
        }
    }

    public static void Start()
    {
        //if (IsRunning())
        //{
        //    Plugin.Logger.LogError("Failed to start server. Server is already running.");
        //    return;
        //}

        if (!File.Exists(GetExecutablePath()))
        {
            Plugin.Logger.LogError("Failed to start server. Could not find server executable.");
            return;
        }
        
        try
        {
            // Run a batch file to start the executable in a separate console window
            string batchFilePath = Path.Combine(GetFolderPath(), "start_server.bat");

            // Write the batch file
            File.WriteAllText(batchFilePath, $"cd /d \"{GetFolderPath()}\"\nstart \"StreamOverlays Server\" \"{GetExecutablePath()}\" --unityPort {_serverPort}");

            // Start the batch file to open the new console window
            Process.Start(batchFilePath);

            Plugin.Logger.LogInfo("Server started successfully in a new console window.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"Failed to start the server: {ex.Message}");
        }
    }

    //public static void Stop()
    //{
    //    if (_serverProcess != null && !_serverProcess.HasExited)
    //    {
    //        try
    //        {
    //            _serverProcess.Kill();
    //            _serverProcess.WaitForExit();

    //            Plugin.Logger.LogInfo("Server stopped successfully.");
    //        }
    //        catch (Exception ex)
    //        {
    //            Plugin.Logger.LogError($"Failed to stop the server: {ex.Message}");
    //        }
    //    }
    //    else
    //    {
    //        Plugin.Logger.LogError("No running server process found to stop.");
    //    }
    //}

    //public static bool IsRunning()
    //{
    //    Process[] processes = Process.GetProcessesByName(ProccessName);

    //    // Check if the server process is already running
    //    return processes.Any(p => !p.HasExited);
    //}

    private static bool IsPacked()
    {
        if (!File.Exists(GetArchivePath()))
        {
            return false;
        }

        if (File.Exists(GetExecutablePath()))
        {
            return false;
        }

        return true;
    }

    private static async Task Unpack()
    {
        string archivePath = GetArchivePath();

        if (!File.Exists(archivePath))
        {
            Plugin.Logger.LogError("Failed to unpack server archive. Archive file was not found.");
            return;
        }

        try
        {
            // Decompress the archive asynchronously
            await Task.Run(() =>
            {
                try
                {
                    // Decompress the archive file to the folder
                    ZipFile.ExtractToDirectory(archivePath, GetFolderPath());

                    // If successful, delete the archive file
                    File.Delete(archivePath);

                    Plugin.Logger.LogInfo("Server files unpacked and archive deleted.");
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogError($"Failed to unpack server archive: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"Failed to decompress archive: {ex.Message}");
        }
    }

    private static string GetFolderPath()
    {
        string pluginFolderPath = Path.GetDirectoryName(Plugin.Instance.Info.Location);
        return Path.Combine(pluginFolderPath, FolderName);
    }

    private static string GetArchivePath()
    {
        return Path.Combine(GetFolderPath(), ExecutableName + ".zip");
    }

    private static string GetExecutablePath()
    {
        return Path.Combine(GetFolderPath(), ExecutableName + ".exe");
    }
}
