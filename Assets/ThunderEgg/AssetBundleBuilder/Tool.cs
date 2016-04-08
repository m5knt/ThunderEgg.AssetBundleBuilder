using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace ThunderEgg.AssetBundleBuilder {

    public class Tool {

        struct P2R {
            public P2R(string platform, string root) {
                Platform = platform;
                Root = root;
            }
            public string Platform;
            public string Root;
        };

        static readonly P2R[] Platform2Folder = new[] {

            // build targets
            new P2R(BuildTarget.StandaloneWindows.ToString(),"Windows"),
            new P2R(BuildTarget.StandaloneWindows64.ToString(),"Windows"),
            new P2R(BuildTarget.StandaloneOSXIntel.ToString(),"OSX"),
            new P2R(BuildTarget.StandaloneOSXIntel64.ToString(),"OSX"),
            new P2R(BuildTarget.StandaloneOSXUniversal.ToString(),"OSX"),
            new P2R(BuildTarget.StandaloneLinux.ToString(),"Linux"),
            new P2R(BuildTarget.StandaloneLinux64.ToString(),"Linux"),
            new P2R(BuildTarget.StandaloneLinuxUniversal.ToString(),"Linux"),
            new P2R(BuildTarget.WebGL.ToString(),"WebGL"),
            new P2R(BuildTarget.WebPlayer.ToString(),"WebPlayer"),
            new P2R(BuildTarget.Android.ToString(),"Android"),
            new P2R(BuildTarget.iOS.ToString(),"iOS"),
            new P2R(BuildTarget.PS4.ToString(),"PS4"),
            new P2R(BuildTarget.XboxOne.ToString(),"XboxOne"),
            new P2R(BuildTarget.WiiU.ToString(),"WiiU"),

            // runtime platforms
            new P2R(RuntimePlatform.WindowsEditor.ToString(),"Windows"),
            new P2R(RuntimePlatform.WindowsPlayer.ToString(),"Windows"),
            new P2R(RuntimePlatform.OSXDashboardPlayer.ToString(),"OSX"),
            new P2R(RuntimePlatform.OSXEditor.ToString(),"OSX"),
            new P2R(RuntimePlatform.OSXPlayer.ToString(),"OSX"),
            new P2R(RuntimePlatform.LinuxPlayer.ToString(),"Linux"),
            new P2R(RuntimePlatform.WebGLPlayer.ToString(),"WebGL"),
            new P2R(RuntimePlatform.OSXWebPlayer.ToString(),"WebPlayer"),
            new P2R(RuntimePlatform.WindowsWebPlayer.ToString(),"WebPlayer"),
            new P2R(RuntimePlatform.Android.ToString(),"Android"),
            new P2R(RuntimePlatform.IPhonePlayer.ToString(),"iOS"),
            new P2R(RuntimePlatform.PS4.ToString(),"PS4"),
            new P2R(RuntimePlatform.XboxOne.ToString(),"XboxOne"),
            new P2R(RuntimePlatform.WiiU.ToString(),"WiiU"),
        };

        public static string Output = "AssetBundles";

        public static string PlatformToRoot(BuildTarget target) {
            var t = target.ToString();
            var tbl = Platform2Folder.FirstOrDefault(_ => _.Platform == t);
            return tbl.Root ?? "Unknown";
        }

        public static string PlatformToRoot(RuntimePlatform target) {
            var t = target.ToString();
            var tbl = Platform2Folder.FirstOrDefault(_ => _.Platform == t);
            return tbl.Root ?? "Unknown";
        }

        public static BuildTarget RootToBuildTarget(string root) {
            if (root == "Unknown") {
                throw new InvalidProgramException("Unknown Platform");
            }
            var target = (BuildTarget)Enum.Parse(typeof(BuildTarget), root);
            return target;
        }
    }
}

