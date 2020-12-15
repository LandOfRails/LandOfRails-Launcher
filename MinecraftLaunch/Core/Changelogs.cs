using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace LandOfRailsLauncher.MinecraftLaunch.Core
{
    public class Changelogs
    {
        private static Dictionary<string, string> changelogUrls = new Dictionary<string, string>()
        {
            { "1.13", "https://feedback.minecraft.net/hc/en-us/articles/360007323492-Minecraft-Java-Edition-1-13-Update-Aquatic-" },
            { "1.14.2", "https://feedback.minecraft.net/hc/en-us/articles/360028919851-Minecraft-Java-Edition-1-14-2" },
            { "1.14.3", "https://feedback.minecraft.net/hc/en-us/articles/360030771451-Minecraft-Java-Edition-1-14-3" },
            { "1.14.4", "https://feedback.minecraft.net/hc/en-us/articles/360030780172-Minecraft-Java-Edition-1-14-4" },
            { "1.15.1", "https://feedback.minecraft.net/hc/en-us/articles/360038054332-Minecraft-Java-Edition-1-15-1" },
            { "1.15.2", "https://feedback.minecraft.net/hc/en-us/articles/360038800232-Minecraft-Java-Edition-1-15-2" }
        };

        public static string[] GetAvailableVersions()
        {
            return changelogUrls.Keys.ToArray();
        }

        public static string GetChangelogUrl(string version)
        {
            var url = "";
            return changelogUrls.TryGetValue(version, out url) ? url : null;
        }

        static Regex articleRegex = new Regex("<article class=\\\"article\\\">(.*)<\\/article>", RegexOptions.Singleline);

        public static string GetChangelogHtml(string version)
        {
            var url = GetChangelogUrl(version);
            if (string.IsNullOrEmpty(url))
                return "";

            var html = "";
            using (var wc = new WebClient())
            {
                html = Encoding.UTF8.GetString(wc.DownloadData(url));
            }

            System.IO.File.WriteAllText("test.txt", html);

            var regResult = articleRegex.Match(html);
            return !regResult.Success ? "" : regResult.Value;
        }
    }
}
