using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace LandOfRailsLauncher.MinecraftLaunch.Core
{
    public class MRule
    {
        static MRule()
        {
            OSName = getOSName();

            if (Environment.Is64BitOperatingSystem)
                Arch = "64";
            else
                Arch = "32";
        }

        public static string OSName { get; private set; }
        public static string Arch { get; private set; }

        private static string getOSName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "windows";
            else
                return "linux";
/*
            var osType = Environment.OSVersion.Platform;

            if (osType == PlatformID.MacOSX)
                return "osx";
            else if (osType == PlatformID.Unix)
                return "linux";
            else
                return "windows";
*/
        }

        public bool CheckOSRequire(JArray arr)
        {
            var require = true;

            foreach (JObject job in arr)
            {
                var action = true; // true : "allow", false : "disallow"
                var containCurrentOS = true; // if 'os' JArray contains current os name

                foreach (var item in job)
                {
                    switch (item.Key)
                    {
                        // action
                        // os (containCurrentOS)
                        case "action":
                            action = (item.Value.ToString() == "allow" ? true : false);
                            break;
                        case "os":
                            containCurrentOS = checkOSContains((JObject)item.Value);
                            break;
                        // etc
                        case "features":
                            return false;
                    }
                }

                if (!action && containCurrentOS)
                    require = false;
                else if (action && containCurrentOS)
                    require = true;
                else if (action && !containCurrentOS)
                    require = false;
            }

            return require;
        }

        static bool checkOSContains(JObject job)
        {
            foreach (var os in job)
            {
                if (os.Key == "name" && os.Value.ToString() == OSName)
                    return true;
            }
            return false;
        }
    }
}
