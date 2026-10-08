using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MvpSystem.ApiFeatures;

namespace MvpSystem;

internal static class DiscordManager
{
    private const int MaxDescriptionLength = 4096;
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromSeconds(10) };
    private static readonly Regex Tag = new("<[^<>]*>", RegexOptions.Compiled);
    private static readonly Regex Markdown = new(@"([\\*_~`|>#\[\]()-])", RegexOptions.Compiled);

    internal static void SendRoundSummary(string richText)
    {
        DiscordSettings settings = MvpSystem.Singleton.Config.Discord;
        if (string.IsNullOrWhiteSpace(settings.WebhookUrl))
            return;

        string description = ToMarkdown(richText).Trim();
        if (description.Length == 0)
        {
            LogManager.Debug("Discord webhook: round summary is empty, nothing to send.");
            return;
        }

        if (description.Length > MaxDescriptionLength)
            description = description.Substring(0, MaxDescriptionLength - 3) + "...";

        string payload = JsonSerializer.Serialize(new
        {
            username = string.IsNullOrWhiteSpace(settings.Username) ? null : settings.Username,
            allowed_mentions = new { parse = Array.Empty<string>() },
            embeds = new[]
            {
                new
                {
                    title = settings.EmbedTitle,
                    description,
                    color = ParseColor(settings.EmbedColor),
                    timestamp = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)
                }
            }
        }, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

        string url = settings.WebhookUrl;
        Task.Run(async () =>
        {
            try
            {
                using StringContent content = new(payload, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await Client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                    LogManager.Error($"Discord webhook failed: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}");
                else
                    LogManager.Debug("Discord webhook: round summary sent.");
            }
            catch (Exception ex)
            {
                LogManager.Error("Discord webhook failed.");
                LogManager.Debug($"Discord webhook exception:\n{ex}");
            }
        });
    }

    private static string ToMarkdown(string richText)
    {
        StringBuilder sb = new();
        int last = 0;
        foreach (Match match in Tag.Matches(richText))
        {
            sb.Append(Markdown.Replace(richText.Substring(last, match.Index - last), @"\$1"));
            string tag = match.Value.Substring(1, match.Value.Length - 2).Trim().TrimStart('/').ToLowerInvariant();
            sb.Append(tag switch
            {
                "b" => "**",
                "i" => "*",
                "u" => "__",
                "s" => "~~",
                _ => ""
            });
            last = match.Index + match.Length;
        }

        sb.Append(Markdown.Replace(richText.Substring(last), @"\$1"));
        return sb.ToString();
    }

    private static int ParseColor(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
            return 0;
        return int.TryParse(hex.Trim().TrimStart('#'), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int color) ? color & 0xFFFFFF : 0;
    }
}