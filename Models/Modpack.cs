using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LandOfRails_Launcher.Models
{
    class Modpack
    {
        public Modpack(string name, string title, string modpackVersion, string minecraftVersion, int organisation, string key, string locationOnServer, string imageUrl)
        {
            this.Name = name;
            this.Title = title;
            this.MinecraftVersion = minecraftVersion;
            this.ModpackVersion = modpackVersion;
            this.Organisation = organisation;
            this.Key = key;
            this.LocationOnServer = locationOnServer;
            this.ImageUrl = imageUrl;
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
    }
}
