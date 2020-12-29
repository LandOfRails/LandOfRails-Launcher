using Newtonsoft.Json;

namespace LandOfRailsLauncher.Models
{
    class Modpack
    {
        public Modpack(string name, string title, string shortcut, string modpackVersion, string minecraftVersion, int organisation, string key, string locationOnServer, string imageUrl, string downloadedImage)
        {
            Name = name;
            Title = title;
            Shortcut = shortcut;
            MinecraftVersion = minecraftVersion;
            ModpackVersion = modpackVersion;
            Organisation = organisation;
            Key = key;
            LocationOnServer = locationOnServer;
            ImageUrl = imageUrl;
            DownloadedImage = downloadedImage;

        }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Shortcut { get; set; }
        public string MinecraftVersion { get; set; }
        public string ModpackVersion { get; set; }

        [JsonIgnore]
        public string CurrentVersion { get; set; }
        public int Organisation { get; set; }
        public string Key { get; set; }
        public string LocationOnServer { get; set; }
        public string ImageUrl { get; set; }
        public string DownloadedImage { get; set; }
    }
}
