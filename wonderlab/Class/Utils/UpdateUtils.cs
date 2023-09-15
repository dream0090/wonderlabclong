﻿using Flurl.Http;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using MinecraftLaunch.Modules.Utils;
using wonderlab.Class.Enum;
using MinecraftLaunch.Modules.Downloaders;

namespace wonderlab.Class.Utils {
    public static class UpdateUtils {
        public const string LocalVersion = "1.2.7";

        private const string BaseUpdateUrl = "https://yangspring114.github.io/wonderlab.update/";

        private const string UpdateUrl = "http://43.136.86.16:14514/api/update/info";

        private const string DownloadUrl = "http://43.136.86.16:14514/api/update/lsaac/download";

        public static readonly string IndexUrl = $"{BaseUpdateUrl}{GlobalResources.LauncherData.IssuingBranch.ToString().ToLower()}/index.json";

        public static readonly string VersionInfoUrl = $"{BaseUpdateUrl}{GlobalResources.LauncherData.IssuingBranch.ToString().ToLower()}/files/window/*/info.json";

        //public static Index Index { get; private set; }

        public static async ValueTask<VersionInfo> GetLatestVersionInfoAsync() {
            try {
                using var responseMessage = await UpdateUrl.GetAsync();
                var json = await responseMessage.GetStringAsync();
                return json.ToJsonEntity<VersionInfo>();                
            }
            catch (Exception ex) {
                $"网络异常，{ex.Message}".ShowMessage("错误");
            }

            return null!;
        }

        public static async void UpdateAsync(VersionInfo info, Action<double> action) {
            if (info.Id.Replace(".","").ToInt32() > LocalVersion.Replace(".", "").ToInt32()) {
                using var downloader = FileDownloader.Build(new() {
                    Url = DownloadUrl,
                    Directory = Directory.GetCurrentDirectory().ToDirectory()!,
                    FileName = "launcher.zip"
                });

                downloader.DownloadProgressChanged += (_, raw) => {
                    action(raw.Progress);
                };

                downloader.BeginDownload();
                var result = await downloader.CompleteAsync();

                if (result.HttpStatusCode is HttpStatusCode.OK) {
                    try {
                        using var zip = ZipFile.OpenRead(result.Result.FullName);
                        zip.ExtractToDirectory(result.Result.Directory!.FullName);

                        await Task.Delay(1000);
                        JsonUtils.WriteLauncherInfoJson();
                        Process.Start(new ProcessStartInfo {
                            FileName = "powershell.exe",
                            Arguments = ArgumentsBuilding(),
                            WorkingDirectory = Directory.GetCurrentDirectory(),
                            UseShellExecute = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                        })!.Dispose();
                    }
                    catch (Exception) { }
                }
            }
        }

        public static string ArgumentsBuilding() {
            int currentPID = Process.GetCurrentProcess().Id;
            string name = Process.GetCurrentProcess().ProcessName, filename = $"{name}.exe";
            return $"Stop-Process -Id {currentPID} -Force;" +
                   $"Wait-Process -Id {currentPID} -ErrorAction SilentlyContinue;" +
                   $"Start-Sleep -Milliseconds 500;" +
                   $"Remove-Item WonderLab.zip -Force;" +
                   $"Remove-Item {filename} -Force;" +
                   $"Rename-Item launcher.exe {filename};" +
                   $"Start-Process {name}.exe -Args updated;";
        }
    }

    public record Index {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("latest")]
        public string Latest { get; set; }
    }

    public record VersionInfo {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("messages")]
        public IEnumerable<string> UpdateMessage { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("branch")]
        public IssuingBranch Branch { get; set; }
    }

    public class UpdateInfo {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("tag_name")]
        public string Version { get; set; }

        [JsonPropertyName("target_commitish")]
        public string Sha1 { get; set; }

        [JsonPropertyName("prerelease")]
        public bool IsPreview { get; set; }

        [JsonPropertyName("name")]
        public string Branch { get; set; }

        [JsonPropertyName("body")]
        public string Description { get; set; }

        [JsonPropertyName("author")]
        public Author Author { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedTime { get; set; }

        [JsonPropertyName("assets")]
        public List<Assets> Assets { get; set; }

    }

    public class Author {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("login")]
        public string OwnerName { get; set; }

        [JsonPropertyName("name")]
        public string UserName { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("url")]
        public string UserInfoUrl { get; set; }

        [JsonPropertyName("html_url")]
        public string UserHomeUrl { get; set; }

        [JsonPropertyName("remark")]
        public string Remark { get; set; }

        [JsonPropertyName("followers_url")]
        public string followers_url { get; set; }

        [JsonPropertyName("following_url")]
        public string following_url { get; set; }

        [JsonPropertyName("gists_url")]
        public string gists_url { get; set; }

        [JsonPropertyName("starred_url")]
        public string starred_url { get; set; }

        [JsonPropertyName("subscriptions_url")]
        public string subscriptions_url { get; set; }

        [JsonPropertyName("organizations_url")]
        public string organizations_url { get; set; }

        [JsonPropertyName("repos_url")]
        public string repos_url { get; set; }

        [JsonPropertyName("events_url")]
        public string events_url { get; set; }

        [JsonPropertyName("received_events_url")]
        public string received_events_url { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class Assets {
        [JsonPropertyName("browser_download_url")]
        public string Url { get; set; }

        [JsonPropertyName("name")]
        public string FileName { get; set; }
    }
}

