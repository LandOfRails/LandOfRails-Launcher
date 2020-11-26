using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC;
using DiscordRPC.Logging;

namespace LandOfRailsLauncher.Helper
{
    class DiscordRPC
    {
        public DiscordRpcClient client;
        private static RichPresence presence = new RichPresence()
        {
            Assets = new Assets()
        };

        public DiscordRPC()
        {
            client = new DiscordRpcClient("493078444939411457");
            client.Logger = new ConsoleLogger() { Level = LogLevel.Info };
            client.Initialize();
            SetIdle();
        }

        public void SetIdle()
        {
            presence.Assets.LargeImageKey = "lor_1024_klein_neu";
            presence.Assets.LargeImageText = "LandOfRails Launcher";
            presence.Details = "Kommt noch...";
            presence.State = "In Launcher";
            presence.Timestamps = Timestamps.Now;
            client.SetPresence(presence);
        }
    }
}
