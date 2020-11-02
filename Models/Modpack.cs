using Newtonsoft.Json;

namespace LandOfRails_Launcher.Models
{
    class Modpack
    {
        public Modpack(string name, string title, string modpackVersion, string minecraftVersion, int organisation, string key, string locationOnServer, string imageUrl, string downloadedImage)
        {
            Name = name;
            Title = title;
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
