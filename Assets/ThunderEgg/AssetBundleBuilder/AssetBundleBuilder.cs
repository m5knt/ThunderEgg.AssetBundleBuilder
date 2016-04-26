using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ThunderEgg.AssetBundleBuilder {

    public class AssetBundleBuilder {

        public static string Output = "AssetBundles";
        public static string Unknown = "Unknown";

        struct RP2R {
            public RP2R(RuntimePlatform platform, string root) {
                Platform = platform;
                Root = root;
            }
            public RuntimePlatform Platform;
            public string Root;
        };

        static readonly RP2R[] RuntimePlatform2Root = new[] {
            // runtime platforms
            new RP2R(RuntimePlatform.WindowsEditor, "Windows"),
            new RP2R(RuntimePlatform.WindowsPlayer, "Windows"),
            new RP2R(RuntimePlatform.OSXDashboardPlayer, "OSX"),
            new RP2R(RuntimePlatform.OSXEditor, "OSX"),
            new RP2R(RuntimePlatform.OSXPlayer, "OSX"),
            new RP2R(RuntimePlatform.LinuxPlayer, "Linux"),
            new RP2R(RuntimePlatform.WebGLPlayer, "WebGL"),
            new RP2R(RuntimePlatform.OSXWebPlayer, "WebPlayer"),
            new RP2R(RuntimePlatform.WindowsWebPlayer, "WebPlayer"),
            new RP2R(RuntimePlatform.tvOS, "tvOS"),
            new RP2R(RuntimePlatform.Android, "Android"),
            new RP2R(RuntimePlatform.IPhonePlayer, "iOS"),
            new RP2R(RuntimePlatform.PS4, "PS4"),
            new RP2R(RuntimePlatform.XboxOne, "XboxOne"),
            new RP2R(RuntimePlatform.WiiU, "WiiU"),
        };

        public static string GetRoot(RuntimePlatform target) {
            var tbl = RuntimePlatform2Root
                .FirstOrDefault(_ => _.Platform == target);
            return tbl.Root ?? "Unknown";
        }

#if UNITY_EDITOR

        struct BT2R {
            public BT2R(BuildTarget target, string root) {
                Target = target;
                Root = root;
            }
            public BuildTarget Target;
            public string Root;
        };

        static readonly BT2R[] BuildTarget2Root = new[] {
            // build targets
            new BT2R(BuildTarget.StandaloneWindows, "Windows"),
            new BT2R(BuildTarget.StandaloneWindows64, "Windows"),
            new BT2R(BuildTarget.StandaloneOSXIntel, "OSX"),
            new BT2R(BuildTarget.StandaloneOSXIntel64, "OSX"),
            new BT2R(BuildTarget.StandaloneOSXUniversal, "OSX"),
            new BT2R(BuildTarget.StandaloneLinux, "Linux"),
            new BT2R(BuildTarget.StandaloneLinux64, "Linux"),
            new BT2R(BuildTarget.StandaloneLinuxUniversal, "Linux"),
            new BT2R(BuildTarget.WebGL, "WebGL"),
            new BT2R(BuildTarget.WebPlayer, "WebPlayer"),
            new BT2R(BuildTarget.tvOS, "tvOS"),
            new BT2R(BuildTarget.Android, "Android"),
            new BT2R(BuildTarget.iOS, "iOS"),
            new BT2R(BuildTarget.PS4, "PS4"),
            new BT2R(BuildTarget.XboxOne, "XboxOne"),
            new BT2R(BuildTarget.WiiU, "WiiU"),
        };

        public static string GetRoot(BuildTarget target) {
            var tbl = BuildTarget2Root
                .FirstOrDefault(_ => _.Target == target);
            return tbl.Root ?? Unknown;
        }

        public static BuildTarget RootToBuildTarget(string root) {
            if (root == Unknown) {
                throw new InvalidProgramException("Unknown Platform");
            }
            var tbl = BuildTarget2Root
                .FirstOrDefault(_ => _.Root == root);
            return tbl.Target;
        }
#endif
    }
}

