﻿using Newtonsoft.Json;

namespace LandOfRails_Launcher.MinecraftLaunch.Core
{
    public class MProfileMetadata
    {
        /// <summary>
        /// true : Mojang Server Profile, False : Local profile
        /// </summary>
        public bool IsWeb = true;

        [JsonProperty("id")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public MProfileType MType { get; set; }

        [JsonProperty("releaseTime")]
        public string ReleaseTime { get; set; }

        [JsonProperty("url")]
        public string Path { get; set; }

        public override bool Equals(object obj)
        {
            var info = obj as MProfileMetadata;

            if (info != null)
                return info.Name.Equals(this.Name);
            else if (obj is string)
                return info.Name.Equals(obj.ToString());
            else
                return base.Equals(obj);
        }

        public override string ToString()
        {
            return this.Type + " " + this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
