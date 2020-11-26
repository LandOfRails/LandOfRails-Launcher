using System;

namespace LandOfRailsLauncher.MinecraftLaunch.Core
{
    public enum MFile { Runtime, Library, Resource, Minecraft };

    public class DownloadFileChangedEventArgs : EventArgs
    {
        public MFile FileKind;
        public string FileName;
        public int TotalFileCount;
        public int ProgressedFileCount;
    }
}
